using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Net;
using Server.Net.Packets;
using Server.Services;
using Shared;
using Shared.Enums;

namespace Server.Models;

public class Esp32Device
{
    protected readonly Esp32ServerService ESP32ServerService;
    protected List<ESPBluetoothDevice> BluetoothDevices = new();
    protected bool IsBusy = false;


    protected bool IsScanning = false;


    public Esp32Device(TcpClient client, Esp32ServerService serverService)
    {
        Client = client;
        ESP32ServerService = serverService;
    }

    public string? Uuid { get; set; }
    public TcpClient Client { get; set; }

    public async Task Initialize()
    {
        Uuid = await GetUuid();
    }

    public void OnEvent(Esp32Event esp32Event)
    {
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
        await stream.WriteAsync(requestBytes, cancellationToken);
        return await ESP32ServerService.WaitForResponse(request.Id, TimeSpan.FromSeconds(30), cancellationToken);
    }

    // this one have watchdog
    public async Task<RpcResponse> SendRpcRequestSafe(string method, Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var response = SendRpcRequest(method, parameters, cancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        var task = await Task.WhenAny(response, timeout);
        if (task == timeout)
        {
            throw new TimeoutException();
        }
        else if (task == response)
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
        if (IsScanning) throw new InvalidOperationException("Already scanning");
        IsScanning = true;
        try
        {
            var response = await SendRpcRequestSafe("bluetooth_start_scan");
            return response.Result! ?? throw new InvalidOperationException();
        }
        finally
        {
            IsScanning = false;
        }
    }

    public async Task<JObject> GetInfo()
    {
        var response = await SendRpcRequestSafe("get_info");
        return (JObject) response.Result! ?? throw new InvalidOperationException();
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


    public partial class ServiceDatum
    {
        [JsonProperty("data")] public long[] Data { get; set; }

        [JsonProperty("uuid")] public string Uuid { get; set; }
    }
}