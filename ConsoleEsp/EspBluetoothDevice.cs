using System.Text;
using Newtonsoft.Json;
using Server.Net.Packets;
using Server.Net.Requests;
using Shared;

namespace ConsoleEsp;

public class EspBluetoothDevice : IBluetoothDevice
{
    [JsonProperty("raw_data")] public byte[] raw_data;

    [JsonProperty("name")] public string Name { get; set; } = "Unknown";

    public HashSet<string> Services { get; set; } = new();
    public HashSet<string> Characteristics { get; set; } = new();

    public Func<RpcRequest, Task<RpcResponse>> SendRpcRequest { get; set; } =
        request => throw new NotImplementedException();

    public void Dispose()
    {
    }

    [JsonProperty("address")] public string Address { get; set; }

    public byte[] RawData { get; set; }

    public Task<bool> HasService(string serviceUuid)
    {
        return Task.FromResult(true);
        return Task.FromResult(Services.Contains(serviceUuid));
    }

    public Task<bool> HasCharacteristic(string serviceUuid, string characteristicUuid)
    {
        return Task.FromResult(true);
        return Task.FromResult(Characteristics.Contains(characteristicUuid));
    }

    public async Task<byte[]> ReadCharacteristic(string serviceUuid, string characteristicUuid)
    {
        var request = new ReadCharacteristicRequest(Address, serviceUuid, characteristicUuid);
        var response = await SendRpcRequest(request);
        var hex = response.GetResult<string>();
        if (hex == null)
        {
            throw new Exception("Failed to read characteristic, response was null");
        }

        return RpcRequest.HexToBytes(hex);
    }

    public Task WriteCharacteristic(string serviceUuid, string characteristicUuid, byte[] data)
    {
        var request = new WriteCharacteristicRequest(Address, serviceUuid, characteristicUuid, data);
        return SendRpcRequest(request);
    }

    public Task SubscribeCharacteristic(string serviceUuid, string characteristicUuid, Action<byte[]> onData)
    {
        var request = new SubscribeCharacteristicRequest(Address, serviceUuid, characteristicUuid);
        return SendRpcRequest(request);
    }

    public Task OnDisconnected(Action onDisconnected)
    {
        return Task.CompletedTask;
    }

    public Task<string?> GetName()
    {
        return Task.FromResult(Name);
    }
}