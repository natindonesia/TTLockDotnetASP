using Shared.Enums;

namespace Shared.Api.Commands;

public class CheckUserTimeCommand : AbstractCommand
{
    private string endDate;
    private uint? lockFlagPos;
    private string startDate;
    private uint? uid;

    public CheckUserTimeCommand()
    {
    }

    public CheckUserTimeCommand(byte[]? data) : base(data)
    {
    }

    public override void ProcessData()
    {
    }

    public override byte[] Build()
    {
        if (uid.HasValue && !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate) && lockFlagPos.HasValue)
        {
            var data = new byte[17]; // 5+5+3+4
            DateTimeToBuffer(startDate).CopyTo(data, 0);
            BitConverter.GetBytes((uint) lockFlagPos.Value).CopyTo(data, 9); // overlap first byte
            DateTimeToBuffer(endDate).CopyTo(data, 5);
            BitConverter.GetBytes((uint) uid.Value).CopyTo(data, 13);
            return data;
        }

        return new byte[0];
    }

    public void SetPayload(uint uid, string startDate, string endDate, uint lockFlagPos)
    {
        this.uid = uid;
        this.startDate = startDate;
        this.endDate = endDate;
        this.lockFlagPos = lockFlagPos;
    }

    public int GetPsFromLock()
    {
        if (Data != null)
            return BitConverter.ToInt32(Data, 0);
        else
            return -1;
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