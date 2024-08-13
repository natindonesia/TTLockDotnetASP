using Newtonsoft.Json;

namespace Server.Net.Packets;

public class RpcRequest
{
    [JsonProperty("id")] public ulong Id { get; set; }

    [JsonProperty("method")] public string Method { get; set; }

    [JsonProperty("params")] public Dictionary<string, object> Params { get; set; }


    public static byte[] HexToBytes(string hex)
    {
        if (hex.Length % 2 == 1)
            throw new Exception("The binary key cannot have an odd number of digits");

        byte[] arr = new byte[hex.Length >> 1];

        for (int i = 0; i < hex.Length >> 1; ++i)
        {
            arr[i] = (byte) ((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
        }

        return arr;
    }

    public static string BytesToHex(byte[] bytes)
    {
        char[] c = new char[bytes.Length * 2];
        byte b;
        for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
        {
            b = ((byte) (bytes[bx] >> 4));
            c[cx] = (char) (b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            b = ((byte) (bytes[bx] & 0x0F));
            c[++cx] = (char) (b > 9 ? b + 0x37 + 0x20 : b + 0x30);
        }

        return new string(c);
    }


    public static int GetHexVal(char hex)
    {
        int val = (int) hex;
        //For uppercase A-F letters:
        //return val - (val < 58 ? 48 : 55);
        //For lowercase a-f letters:
        //return val - (val < 58 ? 48 : 87);
        //Or the two combined, but a bit slower:
        return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
    }
}