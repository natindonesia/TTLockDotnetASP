namespace Shared;

public abstract class BluetoothDevice : IDisposable
{
    public byte[] ManufacturerData { get; set; }


    public abstract Task<bool> HasService(string serviceUuid);
    public abstract Task<bool> HasCharacteristic(string serviceUuid, string characteristicUuid);

    public abstract Task<byte[]> ReadCharacteristic(string serviceUuid, string characteristicUuid);

    public abstract Task WriteCharacteristic(string serviceUuid, string characteristicUuid, byte[] data);

    public abstract Task SubscribeCharacteristic(string serviceUuid, string characteristicUuid, Action<byte[]> onData);

    public abstract void Dispose();
}
