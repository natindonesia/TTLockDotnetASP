using Shared.Enums;

namespace Shared.Api.Commands;

public abstract class AbstractCommand
{
    public readonly byte[]? Data;
    public CommandResponse Response;

    public AbstractCommand()
    {
        Response = CommandResponse.UNKNOWN;
    }

    public AbstractCommand(byte[]? data)
    {
        if (data == null || data.Length < 2)
        {
            Response = CommandResponse.UNKNOWN;
            return;
        }

        Data = data;
        try
        {
            this.Response = (CommandResponse) ((sbyte) Data[1]);
        }
        catch
        {
            this.Response = CommandResponse.UNKNOWN;
        }


        this.Data = data[2..];
    }

    public abstract void ProcessData();

    public abstract byte[] Build();

    public abstract CommandType GetCommandType();
}