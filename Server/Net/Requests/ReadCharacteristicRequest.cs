using Server.Net.Packets;

namespace Server.Net.Requests;

public class ReadCharacteristicRequest : RpcRequest
{
    public ReadCharacteristicRequest(string address, string serviceUuid, string characteristicUuid)
    {
        Method = "ble_read_characteristic";
        Params = new Dictionary<string, object>
        {
            {"address", address},
            {"service_uuid", Guid.Parse(serviceUuid)},
            {"characteristic_uuid", Guid.Parse(characteristicUuid)}
        };
    }
}