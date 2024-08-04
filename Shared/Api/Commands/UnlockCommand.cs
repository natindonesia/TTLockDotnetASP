using Shared.Enums;

namespace Shared.Api.Commands;

public class UnlockCommand : AbstractCommand
{
    public byte? BatteryCapacity;
    public string DateTime;
    public int? Sum;
    public uint? Uid;
    public uint? UniqueId;

    public UnlockCommand()
    {
    }

    public UnlockCommand(byte[]? data) : base(data)
    {
    }

    public override void ProcessData()
    {
        if (Data == null || Data.Length <= 0) return;
        BatteryCapacity = Data[0];
        if (Data.Length < 15) return;
        Uid = BitConverter.ToUInt32(Data, 1);
        UniqueId = BitConverter.ToUInt32(Data, 5);

        int year = 2000 + Data[9];
        int month = Data[10] - 1;
        int day = Data[11];
        int hour = Data[12];
        int minute = Data[13];
        int second = Data[14];

        DateTime dateObj = new DateTime(year, month, day, hour, minute, second);
        DateTime = dateObj.ToString("yyMMddHHmmss");
    }

    public override byte[] Build()
    {
        if (!Sum.HasValue) return new byte[0];
        byte[] data = new byte[8];
        BitConverter.GetBytes((uint) Sum.Value).CopyTo(data, 0);
        BitConverter.GetBytes((uint) DateTimeOffset.UtcNow.ToUnixTimeSeconds()).CopyTo(data, 4);
        return data;
    }

    public void SetSum(int psFromLock, int unlockKey)
    {
        Sum = psFromLock + unlockKey;
    }

    public int GetBatteryCapacity()
    {
        return BatteryCapacity.HasValue ? BatteryCapacity.Value : -1;
    }


    public override CommandType GetCommandType()
    {
        return CommandType.COMM_UNLOCK;
    }
}