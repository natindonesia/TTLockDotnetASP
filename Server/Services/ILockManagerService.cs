using Shared;

namespace Server.Services;

public interface ILockManagerService
{
    IAsyncEnumerable<TTDevice> GetDevicesAsync();
    Task<TTDevice?> GetDeviceAsync(string macAddress);
}