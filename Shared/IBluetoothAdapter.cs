namespace Shared;

public interface IBluetoothAdapter
{
    public abstract Task<IEnumerable<IBluetoothDevice>> DiscoverDevices();

    public abstract Task<IBluetoothDevice> ConnectToDevice(string address);

    public abstract void Dispose();
}