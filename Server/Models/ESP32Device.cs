using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Server.Models;

public class Esp32Device
{

    public string? Uuid { get; set; }
    public TcpClient Client { get; set; }

    protected Task ReadingTask { get; set; }

    public Esp32Device(TcpClient client)
    {

        Client = client;
    }

    public async Task Initialize()
    {
        Uuid = await GetUuid();
    }

    public async Task<RpcResponse> WaitForResponse(CancellationToken cancellationToken, ulong id)
    {
        var stream = Client.GetStream();

        var buffer = new byte[8192];

        while (!cancellationToken.IsCancellationRequested)
        {
            var readTask = stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            var bytesRead = await readTask;
            if (bytesRead == 0)
            {
                break;
            }
            var data = buffer[..bytesRead];
            var responseJson = Encoding.UTF8.GetString(data);
            RpcResponse? response = null;
            try
            {
                 response = JsonConvert.DeserializeObject<RpcResponse>(responseJson);
            }catch(JsonException)
            {
                continue;
            }

            if(response == null)
            {
                continue;
            }
            if (response.Id == id)
            {
                return response;
            }

        }
        throw new OperationCanceledException();
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
        var requestBytes = Encoding.UTF8.GetBytes(requestJson);
        var stream = Client.GetStream();
        await stream.WriteAsync(requestBytes, cancellationToken);
        return await WaitForResponse(cancellationToken, request.Id);
    }

    public async Task<string> GetUuid()
    {
        var response = await SendRpcRequest("get_uuid");
        return response.Result?.ToString() ?? throw new InvalidOperationException();
    }

    public async Task<Dictionary<string, string>> BluetoothScan()
    {
        var response = await SendRpcRequest("bluetooth_start_scan");
        return (Dictionary<string, string>)response.Result! ?? throw new InvalidOperationException();
    }

    public override string ToString()
    {
        return $"UUID: {Uuid}";
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