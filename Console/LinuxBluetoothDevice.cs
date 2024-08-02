using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using Shared;
using Shared.Exceptions;

namespace Console;

public class LinuxBluetoothDevice : IBluetoothDevice
{
    public static readonly TimeSpan timeout = TimeSpan.FromSeconds(5);
    protected bool Connected = false;

    protected Device Device;
    public string Address { get; set; } = "";
    public byte[] RawData { get; set; } = new byte[0];


    public async Task<bool> HasService(string serviceUuid)
    {
        await Connect();
        var services = await Device.GetServicesAsync();
        foreach (var service in services)
        {
            var uuid = await service.GetUUIDAsync();
            if (uuid == serviceUuid)
            {
                return true;
            }
        }

        return false;
    }

    public async Task<bool> HasCharacteristic(string serviceUuid, string characteristicUuid)
    {
        await Connect();
        var service = await Device.GetServiceAsync(serviceUuid);
        if (service == null)
        {
            return false;
        }

        var characteristics = await service.GetCharacteristicsAsync();
        foreach (var characteristic in characteristics)
        {
            var uuid = await characteristic.GetUUIDAsync();
            if (uuid == characteristicUuid)
            {
                return true;
            }
        }

        return false;
    }

    public async Task<byte[]> ReadCharacteristic(string serviceUuid, string characteristicUuid)
    {
        await Connect();
        var service = await Device.GetServiceAsync(serviceUuid);
        if (service == null)
        {
            throw new ServiceNotFoundException(serviceUuid);
        }

        var characteristic = await service.GetCharacteristicAsync(characteristicUuid);
        if (characteristic == null)
        {
            throw new CharacteristicNotFoundException(characteristicUuid);
        }

        return await characteristic.ReadValueAsync(new Dictionary<string, object>());
    }

    public async Task WriteCharacteristic(string serviceUuid, string characteristicUuid, byte[] data)
    {
        await Connect();
        var service = await Device.GetServiceAsync(serviceUuid);
        if (service == null)
        {
            throw new ServiceNotFoundException(serviceUuid);
        }

        var characteristic = await service.GetCharacteristicAsync(characteristicUuid);
        if (characteristic == null)
        {
            throw new CharacteristicNotFoundException(characteristicUuid);
        }

        await characteristic.WriteValueAsync(data, new Dictionary<string, object>());
    }

    public async Task SubscribeCharacteristic(string serviceUuid, string characteristicUuid, Action<byte[]> onData)
    {
        await Connect();
        var service = await Device.GetServiceAsync(serviceUuid);
        if (service == null)
        {
            throw new ServiceNotFoundException(serviceUuid);
        }

        var characteristic = await service.GetCharacteristicAsync(characteristicUuid);
        if (characteristic == null)
        {
            throw new CharacteristicNotFoundException(characteristicUuid);
        }

        characteristic.Value += async (sender, args) =>
        {
            var value = args.Value;
            onData(value);
        };

        await characteristic.StartNotifyAsync();
    }

    public void Dispose()
    {
        Device.Dispose();
    }

    public static async Task<LinuxBluetoothDevice> FromDevice(Device device)
    {
        LinuxBluetoothDevice ble = new LinuxBluetoothDevice();
        var properties = await device.GetAllAsync();
        ble.Address = properties.Address;
        if (ble.Address == "F2:C1:AD:4C:AE:FD")
        {
            ble.RawData = new byte[]
            {
                2, 1, 6, 2, 10, 191, 3, 2, 16, 25, 18, 255, 5, 3, 2, 28, 90, 176, 0, 244, 249, 83, 101, 253, 174, 76,
                173, 193, 242, 11, 9, 68, 48, 49, 95, 102, 100, 97, 101, 52, 99, 5, 18, 20, 0, 36, 0
            };
        }

        ble.Device = device;
        return ble;
    }

    protected async Task Connect()
    {
        if (!Connected)
        {
            Device.Disconnected += async (sender, args) => { Connected = false; };
            await Device.ConnectAsync();
            await Device.WaitForPropertyValueAsync("Connected", value: true, timeout);
            await Device.WaitForPropertyValueAsync("ServicesResolved", value: true, timeout);
        }
    }
}