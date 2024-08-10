using Server.Models;

namespace Server.Services;

/**
 * Handle ESP32 as devices, also notify devices of changes
 */
public interface IEspDeviceManagerService : IHostedService
{
    public Task<IEnumerable<IEspDevice>> GetDevicesAsync(CancellationToken cancellationToken = default);

    public Task<IEspDevice?> GetDeviceAsync(Guid uuid, CancellationToken cancellationToken = default);
}