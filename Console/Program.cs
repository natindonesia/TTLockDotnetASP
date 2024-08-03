using Console;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using Shared;
using Shared.Enums;

Adapter? adapter = (await BlueZManager.GetAdaptersAsync()).FirstOrDefault();
if (adapter == null)
{
    System.Console.WriteLine("No Bluetooth adapter found.");
    return;
}


adapter.DeviceFound += async (sender, eventArgs) =>
{
    var properties = await eventArgs.Device.GetAllAsync();
    System.Console.WriteLine($"Device found: {properties.Name} ({properties.Address})");
};
await adapter.StartDiscoveryAsync();

System.Console.WriteLine("Press Enter to stop discovery.");
// ReadLine is blocking the thread until the user presses Enter, so we use async Task.Run to keep the event handler running.
await Task.Run(System.Console.ReadLine);


await adapter.StopDiscoveryAsync();

var devices = await adapter.GetDevicesAsync();
List<Device1Properties> deviceProperties = new List<Device1Properties>();
List<TTDevice> ttDevices = new List<TTDevice>();

foreach (var device in devices)
{
    var properties = await device.GetAllAsync();
    deviceProperties.Add(properties);
    var linuxBluetoothDevice = await LinuxBluetoothDevice.FromDevice(device);
    var ttDevice = TTDevice.FromBluetoothDevice(linuxBluetoothDevice);
    if (ttDevice != null && ttDevice.LockType != LockType.UNKNOWN) ttDevices.Add(ttDevice);
}

System.Console.WriteLine("Found " + ttDevices.Count + " TT Devices");
while (true)
{
    foreach (var device in ttDevices)
    {
        try
        {
            await device.ReadBasicInfo();
            if (device.IsInitialized)
            {
                await device.InitLock();
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            System.Console.WriteLine("Retrying...");

            continue;
        }

        System.Console.WriteLine(device);
    }

    await Task.Delay(1000);
}