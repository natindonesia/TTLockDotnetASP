using Newtonsoft.Json;
using Server.Models;

namespace Server.Net.Packets;

public class Esp32Response
{
    public Esp32Device Device { get; set; }

    [JsonProperty("packet_type")]
    public string PacketType { get; set; }

    public DateTime Timestamp { get; set; }



}