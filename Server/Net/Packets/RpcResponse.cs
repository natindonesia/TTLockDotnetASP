using Newtonsoft.Json;
using Server.Models;
using Server.Net.Packets;

namespace Server.Net;

public class RpcResponse : ESP32Response
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("result")]
    public object? Result { get; set; }

    [JsonProperty("error")]
    public string? Error { get; set; }


}