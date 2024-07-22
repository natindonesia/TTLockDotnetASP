using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server.Models;

public class Esp32Device
{

    public string? Uuid { get; set; }
    public TcpClient Client { get; set; }
    public StreamReader? Reader { get; set; }
    protected Task? ReadingTask { get; set; }

    protected ConcurrentDictionary<ulong, TaskCompletionSource<RpcResponse>> ResponseTasks { get; set; } =
        new ConcurrentDictionary<ulong, TaskCompletionSource<RpcResponse>>();

    public Esp32Device(TcpClient client)
    {

        Client = client;
    }

    public async Task Initialize()
    {
        ReadingTask = Task.Run(ResponseLoop);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        Uuid = await GetUuid();
    }

    protected async Task OnEvent(Event @event)
    {
        //System.Console.WriteLine(@event.Name);
    }

    protected async Task ResponseLoop()
    {
        var stream = Client.GetStream();
        if (Reader == null)
        {
            Reader = new StreamReader(stream, Encoding.UTF8, false, 8096, true);
        }

        var reader = Reader;
        while (Client.Connected)
        {
            var responseJson = await reader.ReadLineAsync();
            if (responseJson == null)
            {
                break;
            }

            RpcResponse? response = null;
            try
            {
                response = JsonConvert.DeserializeObject<RpcResponse>(responseJson);
            }
            catch (JsonReaderException)
            {
               continue;
            }
            // not this?;
            if(response == null || (response.Result == null && response.Error == null && response.Id == 0)){
                try
                {
                    var eventObject = JsonConvert.DeserializeObject<Event>(responseJson);
                    if (eventObject != null) await Task.Run(() => OnEvent(eventObject));
                }
                catch (JsonReaderException)
                {
                    continue;
                }
            }
            if (response == null)
            {
                continue;
            }

            if (ResponseTasks.TryGetValue(response.Id, out var taskCompletionSource))
            {
                taskCompletionSource.SetResult(response);
            }

        }
    }

    protected void CheckReadingTask()
    {
        if (ReadingTask == null)
        {
            throw new InvalidOperationException("Device not initialized");
        }

        if (ReadingTask.Exception != null)
        {
            throw ReadingTask.Exception;
        }

        if (ReadingTask.Status != TaskStatus.Running && ReadingTask.Status != TaskStatus.WaitingForActivation)
        {
            throw new InvalidOperationException("Device not initialized: " + ReadingTask.Status);
        }

        if (ReadingTask.IsFaulted)
        {
            throw ReadingTask.Exception ?? throw new InvalidOperationException("Device not initialized");
        }
    }


    public async Task<RpcResponse> SendRpcRequest(string method, Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        parameters ??= new Dictionary<string, object>();
        var request = new RpcRequest
        {
            Id = (ulong) DateTime.Now.Ticks,
            Method = method,
            Params = parameters
        };

        var requestJson = JsonConvert.SerializeObject(request);
        var requestBytes = Encoding.UTF8.GetBytes(requestJson + "\n");
        var stream = Client.GetStream();
        var taskCompletionSource = new TaskCompletionSource<RpcResponse>();
        ResponseTasks.TryAdd(request.Id, taskCompletionSource);
        await stream.WriteAsync(requestBytes, cancellationToken);
        CheckReadingTask();
        var response = await taskCompletionSource.Task;
        ResponseTasks.TryRemove(request.Id, out _);
        return response;
    }

    // this one have watchdog
    public async Task<RpcResponse> SendRpcRequestSafe(string method, Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default){
        var response = SendRpcRequest(method, parameters, cancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        var task = await Task.WhenAny(response, timeout);
        if (task == timeout)
        {
            throw new TimeoutException();
        }else if (task == response)
        {
            return await response;
        }

        throw new InvalidOperationException();
    }


    public async Task<string> GetUuid()
    {
        var response = await SendRpcRequestSafe("get_uuid");
        return response.Result?.ToString() ?? throw new InvalidOperationException();
    }



    public async Task<object> GetBluetoothScan()
    {
        var response = await SendRpcRequestSafe("bluetooth_start_scan");
        return response.Result! ?? throw new InvalidOperationException();
    }

    public async Task<JObject> GetInfo()
    {
        var response = await SendRpcRequestSafe("get_info");
        return (JObject)response.Result! ?? throw new InvalidOperationException();
    }

    public override string ToString()
    {
        return $"UUID: {Uuid}";
    }


    public class Event
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Data}";

        }
    }

    public class BluetoothDevice
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("adv_flags")]
        public string AdvFlags { get; set; }

        [JsonProperty("adv_type")]
        public string AdvType { get; set; }

        [JsonProperty("manufacture_data")]
        public long[] ManufactureData { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rssi")]
        public long Rssi { get; set; }

        [JsonProperty("service_data")]
        public ServiceDatum[] ServiceData { get; set; }

        [JsonProperty("service_uuids")]
        public string[] ServiceUuids { get; set; }

        [JsonProperty("raw_data")]
        public byte[] RawData { get; set; }
    }

    public partial class ServiceDatum
    {
        [JsonProperty("data")]
        public long[] Data { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }
    }
    public override bool Equals(object obj)
    {
        if (obj is Esp32Device device)
        {
            return device.Uuid == Uuid;
        }
        return false;
    }

    public class RpcRequest
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public Dictionary<string, object> Params { get; set; }
    }

    public class RpcResponse
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("result")]
        public object? Result { get; set; }

        [JsonProperty("error")]
        public string? Error { get; set; }
    }
}