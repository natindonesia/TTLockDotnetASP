using Server.Models;
using Shared;

namespace Server.Services;

public interface IBluetoothDeviceManagerService
{
    IAsyncEnumerable<IBluetoothDevice> GetDevicesAsync();
    Task<IBluetoothDevice> GetDeviceAsync(string macAddress);
}