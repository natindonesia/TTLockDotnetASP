using System.Text;

namespace Shared.Utils;

public class DigitUtil
{
    private static Random rand = new Random();

    /**
     * generate dynamic password, the first position is zero
     *
     * @param length
     * @return
     */
    public static String generateDynamicPassword(int length)
    {
        byte[] bytes = new byte[length];
        bytes[0] = 48;
        for (int i = 1; i < length; i++)
        {
            double r = rand.NextDouble() * 10;
            if (r >= 10)
            {
                r = 9;
            }

            bytes[i] = (byte) (r + 48);
        }

        return System.Text.Encoding.Default.GetString(bytes);
    }

    public static byte[] shortToByteArray(short value)
    {
        byte[] shortByteArray = new byte[2];
        for (int i = 1; i >= 0; i--)
        {
            shortByteArray[i] = (byte) value;
            value >>= 8;
        }

        return shortByteArray;
    }

    public static byte generateRandomByte()
    {
        return (byte) rand.Next(0, 256);
    }

    public static string byteArrayToHexString(byte[] data)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in data)
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }

    public static short byteArrayToShort(byte[] bytes)
    {
        return (short) ((bytes[0] << 8) | (bytes[1] & 0xff));
    }
}