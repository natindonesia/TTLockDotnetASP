using Newtonsoft.Json;
using Shared;

namespace Server.Models;

// ITS STATELESS HAH
public class ESPBluetoothDevice : IBluetoothDevice
{
    [JsonIgnore] public Esp32Device Device;

    public ESPBluetoothDevice(Esp32Device device)
    {
        Device = device;
    }

    [JsonProperty("adv_flags")] public string AdvFlags { get; set; }

    [JsonProperty("adv_type")] public string AdvType { get; set; }

    [JsonProperty("manufacture_data")] public long[] ManufactureData { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("rssi")] public long Rssi { get; set; }

    [JsonProperty("service_data")] public ServiceDatum[] ServiceData { get; set; }

    [JsonProperty("service_uuids")] public string[] ServiceUuids { get; set; }

    [JsonProperty("address")] public string Address { get; set; }


    [JsonProperty("raw_data")] public byte[] RawData { get; set; }

    public async Task<bool> HasService(string serviceUuid)
    {
        return true;
    }

    public async Task<bool> HasCharacteristic(string serviceUuid, string characteristicUuid)
    {
        return true;
    }

    public async Task<byte[]> ReadCharacteristic(string serviceUuid, string characteristicUuid)
    {
        return [];
    }

    public async Task WriteCharacteristic(string serviceUuid, string characteristicUuid, byte[] data)
    {
    }

    public async Task SubscribeCharacteristic(string serviceUuid, string characteristicUuid, Action<byte[]> onData)
    {
    }

    public Task OnDisconnected(Action onDisconnected)
    {
        return Task.CompletedTask;
    }

    public Task<string?> GetName()
    {
        return Task.FromResult(Name);
    }

    public async void Dispose()
    {
    }


    public partial class ServiceDatum
    {
        [JsonProperty("data")] public long[] Data { get; set; }

        [JsonProperty("uuid")] public string Uuid { get; set; }
    }
}