using Newtonsoft.Json;

namespace Server.Net.Packets;

public class Event : Esp32Response
{

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("data")]
    public object Data { get; set; }

    public override string ToString()
    {
        return $"{Name}: {Data}";

    }

    public T? GetData<T>()
    {
        var str = JsonConvert.SerializeObject(Data);
        return JsonConvert.DeserializeObject<T>(str);
    }
}