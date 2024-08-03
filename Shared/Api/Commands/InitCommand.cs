using Shared.Enums;

namespace Shared.Api.Commands;

public class InitCommand : AbstractCommand
{
    public override void ProcessData()
    {
    }

    public override byte[] Build()
    {
        return [];
    }

    public override CommandType GetCommandType()
    {
        return CommandType.COMM_INITIALIZATION;
    }
}