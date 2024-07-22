using System.Net;
using System.Net.Sockets;
using System.Text;
using Server.Models;

namespace Server.Services;

public class ESP32Services : IHostedService
{

    protected List<Esp32Device> devices = new List<Esp32Device>();
    protected ILogger<ESP32Services> logger;
    public ESP32Services(ILogger<ESP32Services> logger)
    {
        this.logger = logger;
    }



    protected async Task AcceptClients(TcpListener listener, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync(cancellationToken);
            logger.LogInformation("Client connected: {ClientRemoteEndPoint}", client.Client.RemoteEndPoint);
            try
            {
                var device = new Esp32Device(client);
                await device.Initialize();
                logger.LogInformation("Device connected: {Device}", device, client.Client.RemoteEndPoint);
                devices.Add(device);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error connecting device");
                client.Close();
            }
        }
    }

    public async Task HandleClient(TcpClient client, CancellationToken cancellationToken)
    {
        var stream = client.GetStream();
        var buffer = new byte[1024];
        while (!cancellationToken.IsCancellationRequested)
        {
            var readTask = stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            var bytesRead = await readTask;
            if (bytesRead == 0)
            {
                break;
            }
            var data = buffer[..bytesRead];
            Console.WriteLine(Encoding.UTF8.GetString(data));
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var listener = new TcpListener(IPAddress.Any, 12346);
        listener.Start();

        logger.LogInformation("Listening for device " + listener.LocalEndpoint);
        var taskAcceptClient = Task.Run(() => AcceptClients(listener, cancellationToken), cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            // process devices
            foreach (var device in devices.ToList())
            {
                try
                {
                    var response = await device.GetInfo();
                    logger.LogInformation("Device {Device} info: {Response}", device, response);
                    var bluetoothScan = await device.GetBluetoothScan();

                    await Task.Delay(10000, cancellationToken);
                }catch(Exception ex)
                {
                    logger.LogError(ex, "Error processing device {Device}", device);
                    device.Client.Close();
                    devices.Remove(device);

                }
            }
            await Task.Delay(1000, cancellationToken);
            if(taskAcceptClient.Status is TaskStatus.RanToCompletion or TaskStatus.Faulted or TaskStatus.Canceled)
            {
                break;
            }
        }

        await taskAcceptClient;

    }

    public async  Task StopAsync(CancellationToken cancellationToken)
    {

    }
}