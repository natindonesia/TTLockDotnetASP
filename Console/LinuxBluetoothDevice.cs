using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using Shared;
using Shared.Exceptions;

namespace Console;

public class LinuxBluetoothDevice : BluetoothDevice
{

    protected Device Device;
    protected bool Connected = false;

    public static readonly TimeSpan timeout = TimeSpan.FromSeconds(5);
    public static async Task<LinuxBluetoothDevice> FromDevice(Device device)
    {
        LinuxBluetoothDevice ble = new LinuxBluetoothDevice();
        var manufacturerData = new byte[0];
        try
        {
            var properties = await device.GetAllAsync();
            manufacturerData = properties.ManufacturerData.FirstOrDefault().Value as byte[];
        }
        catch
        {

        }
        ble.ManufacturerData = manufacturerData ?? new byte[0];
        ble.Device = device;
        return ble;
    }

    protected async Task Connect()
    {
        if (!Connected)
        {
            Device.Disconnected += async (sender, args) =>
            {
                Connected = false;
            };
            await Device.ConnectAsync();
            await Device.WaitForPropertyValueAsync("Connected", value: true, timeout);
            await Device.WaitForPropertyValueAsync("ServicesResolved", value: true, timeout);

        }
    }

    public override async Task<bool> HasService(string serviceUuid)
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

    public override async Task<bool> HasCharacteristic(string serviceUuid, string characteristicUuid)
    {
        await Connect();
        var service = await Device.GetServiceAsync(serviceUuid);
        if(service == null)
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

    public override async Task<byte[]> ReadCharacteristic(string serviceUuid, string characteristicUuid)
    {
        await Connect();
        var service = await Device.GetServiceAsync(serviceUuid);
        if(service == null)
        {
            throw new ServiceNotFoundException(serviceUuid);
        }

        var characteristic = await service.GetCharacteristicAsync(characteristicUuid);
        if(characteristic == null)
        {
            throw new CharacteristicNotFoundException(characteristicUuid);
        }

        return await characteristic.ReadValueAsync(new Dictionary<string, object>());
    }

    public override async Task WriteCharacteristic(string serviceUuid, string characteristicUuid, byte[] data)
    {
        await Connect();
        var service = await Device.GetServiceAsync(serviceUuid);
        if(service == null)
        {
            throw new ServiceNotFoundException(serviceUuid);
        }

        var characteristic = await service.GetCharacteristicAsync(characteristicUuid);
        if(characteristic == null)
        {
            throw new CharacteristicNotFoundException(characteristicUuid);
        }

        await characteristic.WriteValueAsync(data, new Dictionary<string, object>());

    }

    public override async Task SubscribeCharacteristic(string serviceUuid, string characteristicUuid, Action<byte[]> onData)
    {
        await Connect();
        var service = await Device.GetServiceAsync(serviceUuid);
        if(service == null)
        {
            throw new ServiceNotFoundException(serviceUuid);
        }

        var characteristic = await service.GetCharacteristicAsync(characteristicUuid);
        if(characteristic == null)
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

    public override void Dispose()
    {
        Device.Dispose();

    }
}
