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
    protected ConcurrentDictionary<ulong, TaskCompletionSource<RpcResponse>> ResponseTasks { get; set; } = new ConcurrentDictionary<ulong, TaskCompletionSource<RpcResponse>>();

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


    protected async Task ResponseLoop()
    {
        var stream = Client.GetStream();
        if(Reader == null)
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
        if(ReadingTask.Exception != null)
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

    
    public async Task<RpcResponse> SendRpcRequest(string method, Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default)
    {
        parameters ??= new Dictionary<string, object>();
        var request = new RpcRequest
        {
            Id = (ulong)DateTime.Now.Ticks,
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

    public async Task<string> GetUuid()
    {
        var response = await SendRpcRequest("get_uuid");
        return response.Result?.ToString() ?? throw new InvalidOperationException();
    }



    public async Task<JArray> GetBluetoothScan()
    {
        var response = await SendRpcRequest("bluetooth_start_scan");
        return (JArray)response.Result! ?? throw new InvalidOperationException();
    }

    public override string ToString()
    {
        return $"UUID: {Uuid}";
    }

    public partial class BluetoothDevice
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