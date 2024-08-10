using Newtonsoft.Json;
using Server.Net.Packets;

namespace Server.Net.Requests;

public class GetInfoRequest : RpcRequest
{
    [JsonProperty("method")] public string Method { get; set; } = "get_info";
}