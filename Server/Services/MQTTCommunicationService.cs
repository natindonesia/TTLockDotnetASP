using System.Collections;
using System.Collections.Concurrent;
using System.Text;
using Jitbit.Utils;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Server.Models;
using Server.Net.Packets;
using MqttClient = MQTTnet.Server.MqttClient;

namespace Server.Services;

public sealed class MqttCommunicationService : IEspCommunicationManagerService, IHostedService
{
    private const string TopicPrefix = "esp32-ble-proxy/devices/";
    private readonly ILogger<MqttCommunicationService> _logger;
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttClientOptions;

    private readonly ConcurrentQueue<Esp32Response>
        _responseQueue = new(); // only used when no event handler is registered

    private readonly ConcurrentDictionary<ulong, TaskCompletionSource<RpcResponse>> _responseTasks = new();

    public MqttCommunicationService(ILogger<MqttCommunicationService> logger, IOptions<Configuration> options)
    {
        _logger = logger;
        _mqttClientOptions = new MqttClientOptionsBuilder().WithConnectionUri(options.Value.MqttBrokerUri).Build();
        var mqttFactory = new MqttFactory();
        _logger.LogInformation("Connecting to MQTT broker: " + options.Value.MqttBrokerUri);
        _mqttClient = mqttFactory.CreateMqttClient();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _mqttClient.ApplicationMessageReceivedAsync += OnMqttApplicationMessageReceivedEventArgs;
        _mqttClient.DisconnectedAsync += async e =>
        {
            _logger.LogWarning("Disconnected from MQTT broker: " + e);
            await Task.Delay(5000, cancellationToken);
            await _mqttClient.ConnectAsync(_mqttClientOptions, cancellationToken);
        };
        _mqttClient.ConnectedAsync += async e =>
        {
            _logger.LogInformation("Connected to MQTT broker");
            var mqttSubscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter("esp32-ble-proxy/devices/#", MqttQualityOfServiceLevel.ExactlyOnce)
                .Build();

            await _mqttClient.SubscribeAsync(mqttSubscribeOptions, cancellationToken);
        };

        _mqttClient.ConnectingAsync += _ =>
        {
            _logger.LogInformation("Connecting to MQTT broker");
            return Task.CompletedTask;
        };


        await _mqttClient.ConnectAsync(_mqttClientOptions, cancellationToken);
        _logger.LogInformation("Connected to MQTT broker");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Disconnecting from MQTT broker");
        await _mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
    }


    public void RegisterEventHandler(EventHandler<Esp32Response> handler)
    {
        EventHandler += handler;
        // drain the queue
        while (_responseQueue.TryDequeue(out var response)) handler(this, response);
    }

    public Task SendCommand(IEspDevice? device, RpcRequest command)
    {
        var message = new MqttApplicationMessageBuilder()
            .WithContentType("application/json")
            .WithPayload(JsonConvert.SerializeObject(command))
            .WithTopic("esp32-ble-proxy/devices" + (device == null ? "" : "/" + device.Uuid + "/command"))
            .Build();

        return _mqttClient.PublishAsync(message);
    }

    public async Task<RpcResponse> SendCommandAndWaitForResponse(IEspDevice? device, RpcRequest command)
    {
        if (command.Id == 0) command.Id = (ulong) DateTime.Now.Ticks;
        var tcs = new TaskCompletionSource<RpcResponse>();
        _responseTasks[command.Id] = tcs;
        await SendCommand(device, command);

        var watchdog = Task.Delay(5000);
        var response = await Task.WhenAny(tcs.Task, watchdog);
        // be responsible and clean up
        _responseTasks.TryRemove(command.Id, out _);


        if (response == watchdog)
            // no response?
            throw new TimeoutException("Timeout waiting for response");
        return await tcs.Task;
    }

    public bool IsConnected()
    {
        return _mqttClient.IsConnected;
    }

    private event EventHandler<Esp32Response>? EventHandler;

    private async Task OnMqttApplicationMessageReceivedEventArgs(MqttApplicationMessageReceivedEventArgs e)
    {
        try
        {
            _logger.LogInformation("Received message from topic: " + e.ApplicationMessage.Topic);


            if (!e.ApplicationMessage.Topic.StartsWith(TopicPrefix))
            {
                _logger.LogWarning("Received message with invalid topic: " + e.ApplicationMessage.Topic);
                return;
            }

            var uuid = e.ApplicationMessage.Topic.Substring(TopicPrefix.Length);
            // check if valid uuid
            if (!Guid.TryParse(uuid, out _))
            {
                _logger.LogWarning("Received message from invalid topic: " + e.ApplicationMessage.Topic +
                                   " with invalid uuid: " + uuid);
                return;
            }


            var payload = e.ApplicationMessage.ConvertPayloadToString();


            var response = JsonConvert.DeserializeObject<Esp32Response>(payload);
            if (response == null)
            {
                _logger.LogWarning("Received message with invalid payload: " + payload);
                return;
            }

            response.Timestamp = DateTime.Now;
            response.DeviceUuid = uuid;
            response.Data = payload;
            OnEventHandler(response);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
            return;
        }
    }


    private void OnEventHandler(Esp32Response e)
    {
        switch (e.PacketType)
        {
            case Esp32Response.Type.Response:
            {
                var response = JsonConvert.DeserializeObject<RpcResponse>(e.Data);
                if (response == null)
                {
                    _logger.LogWarning("Received response with invalid payload: " + e.Data);
                    return;
                }

                response.CopyFrom(e);
                if (_responseTasks.TryGetValue(response.Id, out var tcs)) tcs.SetResult(response);

                e = response;
                break;
            }
            // no event handler? queue it
            case Esp32Response.Type.Event:
                var res = JsonConvert.DeserializeObject<Esp32Event>(e.Data);
                if (res == null)
                {
                    _logger.LogWarning("Received event with invalid payload: " + e.Data);
                    return;
                }

                res.CopyFrom(e);
                e = res;
                break;
            default:
                throw new ArgumentOutOfRangeException("Invalid packet type: " + e.PacketType);
        }


        if (EventHandler == null)
        {
            if (_responseQueue.Count > 200)
            {
                _logger.LogWarning("Event queue is full, dropping event: " + e);
                _responseQueue.TryDequeue(out _);
            }

            _responseQueue.Enqueue(e);
        }
        else
            EventHandler(this, e);
    }
}