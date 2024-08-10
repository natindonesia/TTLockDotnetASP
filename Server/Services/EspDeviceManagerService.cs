using System.Collections.Concurrent;
using Server.Models;
using Server.Net.Packets;
using Server.Net.Requests;

namespace Server.Services;

public class EspDeviceManagerService : IEspDeviceManagerService
{
    private readonly Dictionary<Guid, IEspDevice> _devices = new();
    private readonly IEspCommunicationManagerService _espCommunicationManagerService;
    private readonly ConcurrentQueue<Esp32Response> _responseQueue = new();
    private Task? _janitorTask = null;
    private ILogger<EspDeviceManagerService> _logger;


    public EspDeviceManagerService(IEspCommunicationManagerService espCommunicationManagerService,
        ILogger<EspDeviceManagerService> logger)
    {
        _espCommunicationManagerService = espCommunicationManagerService;
        _logger = logger;
        _espCommunicationManagerService.RegisterEventHandler(((sender, response) =>
        {
            if (_responseQueue.Count > 100)
            {
                // dequeue the oldest response
                _responseQueue.TryDequeue(out _);
            }

            _responseQueue.Enqueue(response);
        }));
    }


    public async Task<IEnumerable<IEspDevice>> GetDevicesAsync(CancellationToken cancellationToken = default)
    {
        ulong id = (ulong) DateTime.Now.Ticks;
        await _espCommunicationManagerService.SendCommand(null, new GetInfoRequest()
        {
            Id = id
        });

        // we wait for all devices to respond
        await Task.Delay(150, cancellationToken);

        // dequeue all responses that is RpcResponse and has the same id
        var devices = new List<IEspDevice>();

        var guidsToBeRemoved = new HashSet<Guid>();
        guidsToBeRemoved.UnionWith(_devices.Keys);

        while (_responseQueue.TryDequeue(out var response))
        {
            if (response is not RpcResponse rpcResponse || rpcResponse.Id != id) continue;
            if (_devices.TryGetValue(rpcResponse.DeviceUuid, out var dev))
            {
                // you good
                guidsToBeRemoved.Remove(rpcResponse.DeviceUuid);
                continue;
            }

            var device = new Esp32Device(_espCommunicationManagerService, rpcResponse.DeviceUuid);
            _devices[device.Uuid] = device;
            devices.Add(device);
        }

        foreach (var guid in guidsToBeRemoved)
        {
            var device = _devices[guid];
            _devices.Remove(guid);
            device.OnDeviceNotResponding();
        }

        return devices;
    }

    public async Task<IEspDevice?> GetDeviceAsync(Guid uuid, CancellationToken cancellationToken = default)
    {
        var device = await GetDevicesAsync(cancellationToken);
        return device.FirstOrDefault(d => d.Uuid == uuid) ?? null;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _janitorTask = KeepAlive();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _janitorTask?.Dispose();
        _janitorTask = null;
        return Task.CompletedTask;
    }

    private async Task KeepAlive()
    {
        while (true)
        {
            await Task.Delay(5000);
            try
            {
                await GetDevicesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to keep alive");
            }
        }
    }
}