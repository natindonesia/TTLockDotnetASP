namespace Server.Services;

public class ManagerHostedService : IHostedService
{
    private IEspCommunicationManagerService _espCommunicationManagerService;
    private ILogger<ManagerHostedService> _logger;

    public ManagerHostedService(IEspCommunicationManagerService espCommunicationManagerService,
        ILogger<ManagerHostedService> logger)
    {
        _espCommunicationManagerService = espCommunicationManagerService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var startTask = _espCommunicationManagerService.StartAsync(cancellationToken);
        await Task.WhenAll(startTask);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var stopTask = _espCommunicationManagerService.StopAsync(cancellationToken);
        await Task.WhenAll(stopTask);
    }
}