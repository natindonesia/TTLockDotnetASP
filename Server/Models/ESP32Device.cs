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

public class Esp32Device : IEspDevice
{
    private readonly IEspCommunicationManagerService _communicationManagerService;

    public Esp32Device(IEspCommunicationManagerService communicationManagerService, Guid uuid)
    {
        _communicationManagerService = communicationManagerService;
        Uuid = uuid;
    }

    public Guid Uuid { get; set; }

    public void OnDeviceNotResponding()
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

        return await _communicationManagerService.SendCommandAndWaitForResponse(this, request);
    }

    // this one have watchdog
    public async Task<RpcResponse> SendRpcRequestSafe(string method, Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var response = SendRpcRequest(method, parameters, cancellationToken);
        var timeout = Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        var task = await Task.WhenAny(response, timeout);
        if (task == timeout) throw new TimeoutException();
        if (task == response) return await response;


        // should never happen
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
        return (JObject) response.Result! ?? throw new InvalidOperationException();
    }

    public override string ToString()
    {
        return $"UUID: {Uuid}";
    }

    public override bool Equals(object obj)
    {
        if (obj is Esp32Device device) return device.Uuid == Uuid;

        return false;
    }
}