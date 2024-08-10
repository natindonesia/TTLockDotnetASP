namespace Server.Services;

public class ManagerHostedService : IHostedService
{
    private readonly IEspCommunicationManagerService _espCommunicationManagerService;
    private readonly IEspDeviceManagerService _espDeviceManagerService;

    private ILogger<ManagerHostedService> _logger;

    public ManagerHostedService(IEspCommunicationManagerService espCommunicationManagerService,
        ILogger<ManagerHostedService> logger, IEspDeviceManagerService espDeviceManagerService)
    {
        _espCommunicationManagerService = espCommunicationManagerService;
        _logger = logger;
        _espDeviceManagerService = espDeviceManagerService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var startTasks = new List<Task>
        {
            _espCommunicationManagerService.StartAsync(cancellationToken),
            _espDeviceManagerService.StartAsync(cancellationToken)
        };


        await Task.WhenAll(startTasks);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var stopTasks = new List<Task>
        {
            _espCommunicationManagerService.StopAsync(cancellationToken),
            _espDeviceManagerService.StopAsync(cancellationToken)
        };
        await Task.WhenAll(stopTasks);
    }
}