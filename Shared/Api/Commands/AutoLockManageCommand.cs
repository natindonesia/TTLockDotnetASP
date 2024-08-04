using Shared.Enums;

namespace Shared.Api.Commands;

public class AutoLockManageCommand : AbstractCommand
{
    private byte? _batteryCapacity;
    private AutoLockOperate _opType = AutoLockOperate.SEARCH;
    private ushort? _opValue;

    public AutoLockManageCommand()
    {
    }

    public AutoLockManageCommand(byte[]? data) : base(data)
    {
    }

    public override void ProcessData()
    {
        if (Data == null || Data.Length < 4) throw new ArgumentException("Invalid data for AutoLockManageCommand");
        // 0 - battery
        // 1 - opType
        // 2,3 - opValue
        // 4,5 - min value
        // 6,7 - max value
        _batteryCapacity = Data[0];
        _opType = (AutoLockOperate) Data[1];
        if (_opType == AutoLockOperate.SEARCH)
        {
            _opValue = BitConverter.ToUInt16(new byte[] {Data[3], Data[2]}, 0);
        }
        else
        {
            // Handle other cases if needed
        }
    }

    public override byte[] Build()
    {
        if (_opType == AutoLockOperate.SEARCH)
        {
            return new byte[] {(byte) _opType};
        }
        else if (_opValue.HasValue)
        {
            return new byte[]
            {
                (byte) _opType,
                (byte) (_opValue.Value >> 8),
                (byte) _opValue.Value
            };
        }
        else
        {
            return new byte[0];
        }
    }

    public void SetTime(ushort opValue)
    {
        this._opValue = opValue;
        this._opType = AutoLockOperate.MODIFY;
    }

    public int GetTime()
    {
        return _opValue.HasValue ? _opValue.Value : -1;
    }

    public int GetBatteryCapacity()
    {
        return _batteryCapacity.HasValue ? _batteryCapacity.Value : -1;
    }

    public override CommandType GetCommandType()
    {
        return CommandType.COMM_AUTO_LOCK_MANAGE;
    }
}