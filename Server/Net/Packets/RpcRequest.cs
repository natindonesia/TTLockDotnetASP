using Newtonsoft.Json;

namespace Server.Net.Packets;

public class RpcRequest
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("method")]
    public string Method { get; set; }

    [JsonProperty("params")]
    public Dictionary<string, object> Params { get; set; }
}