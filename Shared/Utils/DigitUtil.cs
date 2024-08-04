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

    /// <summary>
    ///  Convert a long value to a date string.
    /// Example (949338000000, "yyMMddHHmmss") -> "0001311800"
    public static string FormatDateFromLong(long startDate, string format)
    {
        DateTime dateTime = new DateTime(startDate);
        return dateTime.ToString(format);
    }

    /// <summary>
    /// Converts a time string in yyMMddHHmmss format to a byte array.
    /// </summary>
    /// <param name="time">The time string in yyMMddHHmmss format.</param>
    /// <returns>A byte array representing the time string.</returns>
    public static byte[] ConvertTimeToByteArray(string time)
    {
        int len = time.Length / 2;
        byte[] values = new byte[len];
        for (int i = 0; i < len; i++)
        {
            string str = time.Substring(i * 2, 2);
            values[i] = Convert.ToByte(str);
        }

        return values;
    }

    public static byte[] IntegerToByteArray(int value)
    {
        byte[] bytes = new byte[4];
        byte[] offset = new byte[] {24, 16, 8, 0};
        for (int i = 0; i < 4; i++)
        {
            bytes[i] = (byte) (value >> offset[i]);
        }

        return bytes;
    }

    /**
     * generate dynamic password, the first position is zero
     *
     * @param length
     * @return
     */
    public static String GenerateDynamicPassword(int length)
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

        return Encoding.Default.GetString(bytes);
    }
}