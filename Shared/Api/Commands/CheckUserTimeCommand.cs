using Shared.Enums;
using Shared.Utils;

namespace Shared.Api.Commands;

public class CheckUserTimeCommand : AbstractCommand
{
    private readonly long _endDate;
    private readonly uint _lockFlagPos;
    private readonly long _startDate;
    private readonly int _uid;

    public CheckUserTimeCommand(int uid, long startDate, long endDate, uint lockFlagPos)
    {
        this._uid = uid;
        this._startDate = startDate;
        this._endDate = endDate;
        this._lockFlagPos = lockFlagPos;
    }

    public CheckUserTimeCommand(byte[]? data) : base(data)
    {
        _endDate = 0;
        _lockFlagPos = 0;
        _startDate = 0;
        _uid = 0;
    }

    public override void ProcessData()
    {
    }

    public override byte[] Build()
    {
        String sDateStr = "0001311800";
        String eDateStr = "9911301800";
        var data = new byte[17]; // 5+5+3+4
        byte[] time = DigitUtil.ConvertTimeToByteArray(sDateStr + eDateStr);
        Buffer.BlockCopy(time, 0, data, 0, 10);
        data[10] = (byte) ((_lockFlagPos >> 16) & 0xFF);
        data[11] = (byte) ((_lockFlagPos >> 8) & 0xFF);
        data[12] = (byte) (_lockFlagPos & 0xFF);
        byte[] uidArray = DigitUtil.IntegerToByteArray(_uid);
        Buffer.BlockCopy(uidArray, 0, data, 13, 4);
        return data;
    }


    public uint GetPsFromLock()
    {
        if (Data != null)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(Data);
            return BitConverter.ToUInt32(Data, 0);
        }
        else
            return 0;
    }

    private byte[] DateTimeToBuffer(string dateTimeStr)
    {
        // Assuming the dateTimeStr is in a specific format e.g. "YYMMDDHHmmss"
        DateTime dateTime;
        if (DateTime.TryParseExact(dateTimeStr, "yyMMddHHmmss", null, System.Globalization.DateTimeStyles.None,
                out dateTime))
        {
            var buffer = new byte[5];
            buffer[0] = (byte) (dateTime.Year - 2000);
            buffer[1] = (byte) dateTime.Month;
            buffer[2] = (byte) dateTime.Day;
            buffer[3] = (byte) dateTime.Hour;
            buffer[4] = (byte) dateTime.Minute;
            return buffer;
        }

        return new byte[0];
    }

    public override CommandType GetCommandType()
    {
        return CommandType.COMM_CHECK_USER_TIME;
    }
}