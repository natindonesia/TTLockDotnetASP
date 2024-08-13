using Shared;

namespace Server.Services;

// what this class do?
// it translates bluetooth device to TTDevice
public class EspLockManagerService : ILockManagerService
{
    private readonly IBluetoothDeviceManagerService _bluetoothDeviceManagerService;


    public EspLockManagerService(IBluetoothDeviceManagerService bluetoothDeviceManagerService)
    {
        _bluetoothDeviceManagerService = bluetoothDeviceManagerService;
    }

    public async IAsyncEnumerable<TTDevice> GetDevicesAsync()
    {
        await foreach (var device in _bluetoothDeviceManagerService.GetDevicesAsync())
        {
            var ttDevice = TTDevice.FromBluetoothDevice(device);
            if (ttDevice != null)
            {
                yield return ttDevice;
            }
        }
    }

    public async Task<TTDevice?> GetDeviceAsync(string macAddress)
    {
        var device = await _bluetoothDeviceManagerService.GetDeviceAsync(macAddress);
        return TTDevice.FromBluetoothDevice(device);
    }
}