using Newtonsoft.Json;

namespace Server.Net.Packets;

public class RpcResponse : Esp32Response
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("result")]
    public object? Result { get; set; }

    [JsonProperty("error")]
    public string? Error { get; set; }


}