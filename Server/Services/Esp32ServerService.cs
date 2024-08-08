using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Server.Models;
using Server.Net;
using Server.Net.Packets;

namespace Server.Services;

public class Esp32ServerService : IHostedService
{
    protected readonly BlockingCollection<Esp32Device> Devices = new();
    protected readonly ILogger<Esp32ServerService> Logger;
    protected readonly BlockingCollection<Esp32Response> Responses = new();

    protected readonly ConcurrentDictionary<ulong, TaskCompletionSource<RpcResponse>> ResponseTasks = new();
    protected readonly TcpListenerService TcpListenerService;


    public Esp32ServerService(IHostApplicationLifetime applicationLifetime, ILogger<Esp32ServerService> logger)
    {
        this.TcpListenerService = new TcpListenerService(logger);
        this.Logger = logger;
        this.EventReceived += (_, @event) =>
        {
            @event.Device.OnEvent(@event); //wtf?
        };
        this.TcpListenerService.ClientConnected += (_, client) =>
        {
            var clientEsp32Device = new Esp32Device(client, this);
            Devices.Add(clientEsp32Device);
            Task.Run(() => HandleClient(clientEsp32Device, CancellationToken.None));
            Task.Run(() => clientEsp32Device.Initialize());
        };
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var task = TcpListenerService.StartAsync(cancellationToken);
        var responseHandlerTask = ResponseHandler(cancellationToken);
        await Task.WhenAny(task, responseHandlerTask);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }

    public event EventHandler<Esp32Event> EventReceived = delegate { };

    public void DisposeDevice(Esp32Device device)
    {
        Devices.TryTake(out device);
        device.Client.Close();
    }

    public async Task HandleClient(Esp32Device client, CancellationToken cancellationToken)
    {
        var stream = client.Client.GetStream();
        var reader = new StreamReader(stream);
        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line == null)
            {
                break;
            }
            // try parse json

            RpcResponse? response = null;
            try
            {
                response = JsonConvert.DeserializeObject<RpcResponse>(line);
            }
            catch (JsonReaderException)
            {
                continue;
            }

            // not this?;
            if (response == null || (response.Result == null && response.Error == null && response.Id == 0))
            {
                try
                {
                    var eventObject = JsonConvert.DeserializeObject<Esp32Event>(line);
                    if (eventObject != null)
                    {
                        eventObject.Timestamp = DateTime.UtcNow;
                        eventObject.Device = client;
                        Responses.Add(eventObject, cancellationToken);
                    }
                }
                catch (JsonReaderException)
                {
                }

                continue;
            }

            response.Timestamp = DateTime.UtcNow;
            response.Device = client;
            Responses.Add(response, cancellationToken);
        }

        Devices.TryTake(out client);
    }

    public async Task<RpcResponse> WaitForResponse(ulong id, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var taskCompletionSource = new TaskCompletionSource<RpcResponse>();
        ResponseTasks.TryAdd(id, taskCompletionSource);
        var task = taskCompletionSource.Task;
        var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cancellationToken));
        if (completedTask == task)
        {
            return await task;
        }
        else
        {
            throw new TimeoutException();
        }
    }

    protected async Task ResponseHandler(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (Responses.Count == 0)
            {
                await Task.Delay(250, cancellationToken);
            }
            else
            {
                await Task.Delay(16, cancellationToken);
            }

            if (!Responses.TryTake(out var response)) continue;

            if (response is Esp32Event eventObject)
            {
                EventReceived?.Invoke(this, eventObject);
                continue;
            }

            if (response is not RpcResponse rpcResponse) continue;
            if (ResponseTasks.TryGetValue(rpcResponse.Id, out var taskCompletionSource))
            {
                taskCompletionSource.SetResult(rpcResponse);
                ResponseTasks.TryRemove(rpcResponse.Id, out _);
                continue;
            }

            // add back
            Responses.Add(response, cancellationToken);
        }
    }
}