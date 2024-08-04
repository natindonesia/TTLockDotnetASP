using System.Text.Json;
using System.Text.Json.Serialization;
using Console;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using Shared;
using Shared.Entity;
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
// loading data
// mkdir data

if (!System.IO.Directory.Exists("data"))
{
    System.IO.Directory.CreateDirectory("data");
}

foreach (var device in ttDevices)
{
    var file = "data/" + device.Address + ".json";
    if (System.IO.File.Exists(file))
    {
        try
        {
            var json = System.IO.File.ReadAllText(file);
            device.LockData = JsonSerializer.Deserialize<TTLockData>(json) ?? throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex);
        }
    }
}

while (true)
{
    foreach (var device in ttDevices)
    {
        try
        {
            if (!device.IsInitialized)
            {
                await device.ReadBasicInfo();
                await device.InitLock();
                // we got the data, let's save it
                var serialized = JsonSerializer.Serialize(device.LockData);
                System.IO.File.WriteAllText("data/" + device.Address + ".json", serialized);
            }
            else
            {
                var serialized = JsonSerializer.Serialize(device.LockData);
                System.Console.WriteLine(device.Address + ": " + serialized);

                await device.Unlock();
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex);
            System.Console.WriteLine("Retrying...");

            continue;
        }

        System.Console.WriteLine(device);
    }

    await Task.Delay(1000);
}