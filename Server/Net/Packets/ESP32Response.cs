using Server.Models;

namespace Server.Net.Packets;

public class ESP32Response
{
    public Esp32Device Device { get; set; }
    public DateTime Timestamp { get; set; }



}