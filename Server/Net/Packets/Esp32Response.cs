using Newtonsoft.Json;
using Server.Models;

namespace Server.Net.Packets;

public class Esp32Response
{
    public enum Type
    {
        Response,
        Event
    }

    public Esp32Device? Device { get; set; }

    public string DeviceUuid { get; set; }

    [JsonProperty("packet_type")] public Type PacketType { get; set; }

    public DateTime Timestamp { get; set; }


    public string Data { get; set; }


    public void CopyFrom(Esp32Response response)
    {
        Device = response.Device;
        DeviceUuid = response.DeviceUuid;
        PacketType = response.PacketType;
        Timestamp = response.Timestamp;
        Data = response.Data;
    }
}