using Shared;

namespace Server.Services;

public class EspBluetoothDeviceManagerService : IBluetoothDeviceManagerService
{
    private readonly IDictionary<string, Guid>
        _adapterAddressToEspGuid = new Dictionary<string, Guid>(); // sticky adapter to ESP 32

    // so an adapter can have many devices
    // and we have many adapter that can potentially have many devices and can overlap
    // this manager must choose which adapter to use
    private readonly IEspDeviceManagerService _espDeviceManagerService;


    public EspBluetoothDeviceManagerService(IEspDeviceManagerService espDeviceManagerService)
    {
        _espDeviceManagerService = espDeviceManagerService;
    }


    public IAsyncEnumerable<IBluetoothDevice> GetDevicesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IBluetoothDevice> GetDeviceAsync(string macAddress)
    {
        throw new NotImplementedException();
    }
}