// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using ConsoleEsp;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Server.Net.Packets;
using Server.Net.Requests;
using Server.Services;
using Shared;
using Shared.Entity;

internal class Program
{
    private const string ScanResult =
        "{\"name\":\"ble_scan_result\",\"packet_type\":\"event\",\"data\":{\"name\":\"D01_fdae4c\",\"address\":\"F2:C1:AD:4C:AE:FD\",\"rssi\":-73,\"adv_type\":\"Ind\",\"adv_flags\":\"AdvFlag(DiscGeneral | BrEdrUnsupported)\",\"raw_data\":[2,1,6,2,10,191,3,2,16,25,18,255,5,3,2,26,87,176,0,244,249,83,101,253,174,76,173,193,242,11,9,68,48,49,95,102,100,97,101,52,99,5,18,20,0,36,0],\"service_uuids\":[\"0x1910\"],\"service_data_list\":[],\"manufacture_data\":[5,3,2,26,87,176,0,244,249,83,101,253,174,76,173,193,242]}}";

    public static readonly ConcurrentQueue<Esp32Response> Queue = new ConcurrentQueue<Esp32Response>();

    public static async Task<Esp32Response?> GetResponseAsync(ulong timeout = 5000)
    {
        var start = DateTime.Now;
        while (Queue.IsEmpty)
        {
            if (DateTime.Now - start > TimeSpan.FromMilliseconds(timeout))
            {
                throw new TimeoutException();
            }

            await Task.Delay(100);
        }

        Queue.TryDequeue(out var response);
        return response;
    }

    public static async Task Main(string[] args)
    {
        var mqttFactory = new MqttFactory();

        using var mqttClient = mqttFactory.CreateMqttClient();
        if (mqttClient == null)
        {
            throw new Exception("Failed to create mqtt client");
        }

        var mqttClientOptions = new MqttClientOptionsBuilder().WithConnectionUri("tcp://192.168.1.8:1883").Build();

        // Setup message handling before connecting so that queued messages
        // are also handled properly. When there is no event handler attached all
        // received messages get lost.
        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Console.WriteLine("Received application message: " + e.ApplicationMessage.Topic);
            if (!e.ApplicationMessage.Topic.StartsWith(MqttCommunicationService.TopicPrefix))
            {
                return Task.CompletedTask;
            }

            var split = e.ApplicationMessage.Topic.Split('/');
            // check if last is Guid
            if (!Guid.TryParse(split[^1], out _))
            {
                return Task.CompletedTask;
            }

            var response = MqttCommunicationService.ParseResponseAsync(e);
            if (response != null)
            {
                Queue.Enqueue(response);
            }

            return Task.CompletedTask;
        };

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(MqttCommunicationService.TopicPrefix + "#",
                MqttQualityOfServiceLevel.ExactlyOnce)
            .Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        // discover devices
        var request = new MqttApplicationMessageBuilder()
            .WithTopic("esp32-ble-proxy/devices")
            .WithPayload(JsonConvert.SerializeObject(new GetInfoRequest()))
            .Build();

        await mqttClient.PublishAsync(request, CancellationToken.None);
        var response = await GetResponseAsync();
        var primaryDevice = response!.DeviceUuid;


        var scanResult = JsonConvert.DeserializeObject(ScanResult);
        var bleDevice = JsonConvert.DeserializeObject<EspBluetoothDevice>(ScanResult);

        if (bleDevice == null)
        {
            throw new Exception("Failed to deserialize device");
        }

        bleDevice.Address = "F2:C1:AD:4C:AE:FD";
        bleDevice.RawData =
        [
            2, 1, 6, 2, 10, 191, 3, 2, 16, 25, 18, 255, 5, 3, 2, 26, 87, 176, 0, 244, 249, 83, 101, 253, 174, 76, 173,
            193, 242, 11, 9, 68, 48, 49, 95, 102, 100, 97, 101, 52, 99, 5, 18, 20, 0, 36, 0
        ];
        bleDevice.SendRpcRequest = async request =>
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("esp32-ble-proxy/devices/" + primaryDevice + "/rpc")
                .WithPayload(JsonConvert.SerializeObject(request))
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);
            var res = await GetResponseAsync();
            if (res == null)
            {
                throw new Exception("Failed to get response");
            }

            if (res is RpcResponse response)
            {
                if (response.Error != null)
                {
                    throw new Exception("RPC: " + response.Error);
                }

                return response;
            }

            throw new Exception("Invalid response: " + res.GetType());
        };

        var videlicet = TTDevice.FromBluetoothDevice(bleDevice);
        if (videlicet == null)
        {
            throw new Exception("Failed to create TTDevice");
        }

        const string savedData =
            "{\"Address\":\"F2:C1:AD:4C:AE:FD\",\"Battery\":0,\"Rssi\":0,\"AutoLockTime\":0,\"LockedStatus\":-1,\"PrivateData\":{\"AesKey\":\"oGp+m/2ctsrfVaWvTyniZQ==\",\"Admin\":{\"AdminPs\":30753695,\"UnlockKey\":760142774},\"AdminPasscode\":\"1602131\",\"PwdInfo\":[]},\"Features\":[0,2,5,10,11,14,16,18,20,21,22,23,24,25,28,29,30]}";
        videlicet.LockData = JsonConvert.DeserializeObject<TTLockData>(savedData) ??
                             throw new Exception("Failed to deserialize lock data");
        await videlicet.Unlock();

        Console.WriteLine("Device: " + videlicet.Name);
    }
}