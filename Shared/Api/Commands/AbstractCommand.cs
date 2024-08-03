using Shared.Enums;

namespace Shared.Api.Commands;

public abstract class AbstractCommand
{
    public readonly byte[]? Data;
    public readonly CommandResponse Response;

    public AbstractCommand() : this(null)
    {
        Response = CommandResponse.UNKNOWN;
    }

    public AbstractCommand(byte[]? data)
    {
        if (data != null)
        {
            Data = new byte[data.Length];
            this.Response = (CommandResponse) Data[1];
        }
    }

    public abstract void ProcessData();

    public abstract byte[] Build();

    public abstract CommandType GetCommandType();
}