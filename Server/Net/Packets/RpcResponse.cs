using Newtonsoft.Json;

namespace Server.Net.Packets;

public class RpcResponse : Esp32Response
{
    [JsonProperty("id")] public ulong Id { get; set; }

    [JsonProperty("result")] public object? Result { get; set; }

    [JsonProperty("error")] public string? Error { get; set; }


    public T? GetResult<T>()
    {
        // try cast ?
        if (Result is T result)
        {
            return result;
        }

        return JsonConvert.DeserializeObject<T>(Data);
    }
}