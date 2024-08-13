using Shared.Enums;
using Shared.Utils;

namespace Shared.Api.Commands;

public class CalibrationTimeCommand : AbstractCommand
{
    protected DateTime _dateTime;

    public CalibrationTimeCommand(DateTime dateTime) : base()
    {
        _dateTime = dateTime;
    }

    public CalibrationTimeCommand(byte[]? data) : base(data)
    {
    }

    public override void ProcessData()
    {
    }

    public override byte[] Build()
    {
        return DigitUtil.ConvertTimeToByteArray(_dateTime);
    }

    public override CommandType GetCommandType()
    {
        return CommandType.COMM_TIME_CALIBRATE;
    }
}