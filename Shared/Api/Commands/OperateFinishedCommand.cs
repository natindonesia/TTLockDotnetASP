using Shared.Enums;

namespace Shared.Api.Commands;

public class OperateFinishedCommand : AbstractCommand
{
    public OperateFinishedCommand()
    {
    }

    public OperateFinishedCommand(byte[]? data) : base(data)
    {
    }

    public override void ProcessData()
    {
    }

    public override byte[] Build()
    {
        return [];
    }

    public override CommandType GetCommandType()
    {
        return CommandType.COMM_GET_ALARM_ERRCORD_OR_OPERATION_FINISHED;
    }
}