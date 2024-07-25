using System.Net;
using System.Net.Sockets;

namespace Server.Services;

public class TcpListenerService : IHostedService
{
    protected readonly List<TcpClient> Clients = new List<TcpClient>();

    protected readonly ILogger Logger;
    public event EventHandler<TcpClient> ClientConnected;

    public TcpListenerService(ILogger logger)
    {
        this.Logger = logger;
    }
    



    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var listener = new TcpListener(IPAddress.Any, 12346);
        listener.Start();
        Logger.LogInformation("Listening on {LocalEndpoint}", listener.LocalEndpoint);
        while (!cancellationToken.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync(cancellationToken);
            Logger.LogInformation("Client connected: {ClientRemoteEndPoint}", client.Client.RemoteEndPoint);
            Clients.Add(client);
            ClientConnected?.Invoke(this, client);
        }

        // cleanup
        listener.Stop();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {

    }
}