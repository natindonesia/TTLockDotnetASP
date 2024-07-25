using Shared;

namespace Server.Models;

public class ESPBluetoothDevice : Esp32Device.BluetoothDevice, IBluetoothDevice
{
    public Esp32Device Device;

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

    public async void Dispose()
    {

    }
}