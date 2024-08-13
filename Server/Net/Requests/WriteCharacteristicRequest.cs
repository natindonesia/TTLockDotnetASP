using Server.Net.Packets;

namespace Server.Net.Requests;

public class WriteCharacteristicRequest : RpcRequest
{
    public WriteCharacteristicRequest(string address, string serviceUuid, string characteristicUuid, byte[] value)
    {
        Method = "ble_write_characteristic";
        Params = new Dictionary<string, object>
        {
            {"address", address},
            {"service_uuid", Guid.Parse(serviceUuid)},
            {"characteristic_uuid", Guid.Parse(characteristicUuid)},
            {"value", BytesToHex(value)}
        };
    }
}