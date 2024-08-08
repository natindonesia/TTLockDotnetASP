namespace Server.Models;

public class Configuration
{
    public Uri MqttBrokerUri { get; set; } = new Uri("mqtt://localhost:1883");
}