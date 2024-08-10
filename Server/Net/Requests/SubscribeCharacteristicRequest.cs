using Server.Net.Packets;

namespace Server.Net.Requests;

public class SubscribeCharacteristicRequest : RpcRequest
{
    public SubscribeCharacteristicRequest(string address, string serviceUuid, string characteristicUuid)
    {
        Method = "ble_subscribe_characteristic";
        Params = new Dictionary<string, object>
        {
            {"address", address},
            {"service_uuid", Guid.Parse(serviceUuid)},
            {"characteristic_uuid", Guid.Parse(characteristicUuid)}
        };
    }
}