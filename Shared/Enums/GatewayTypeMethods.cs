namespace Shared.Enums;

public class GatewayTypeMethods
{



    public static GatewayType GetGatewayType(sbyte[] data) {
        if (data[0] == 0x11 && data[1] == 0x19)
        {
            return GatewayType.G2;
        }
        if (data[0] == 0x11 && data[1] == 0x20) {
            return GatewayType.G4;
        }
        if (data[0] == 0x11 && data[1] == 0x21) {
            return GatewayType.G3;
        }
        return GatewayType.UNKNOWN;
    }
}
