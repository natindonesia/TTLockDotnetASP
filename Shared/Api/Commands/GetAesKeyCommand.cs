using System.Text;
using Shared.Enums;

namespace Shared.Api.Commands;

public class GetAesKeyCommand : AbstractCommand
{
    protected byte[]? AesKey;

    public GetAesKeyCommand() : base()
    {
    }

    public GetAesKeyCommand(byte[]? data) : base(data)
    {
    }

    public override void ProcessData()
    {
        if (Data != null)
        {
            AesKey = Data;
        }
    }

    public void SetAESKey(byte[] aesKey)
    {
        AesKey = aesKey;
    }

    public override byte[] Build()
    {
        if (AesKey == null)
        {
            return Encoding.UTF8.GetBytes(Const.VENDOR);
        }

        var data = new byte[AesKey.Length + 2];
        data[0] = (byte) GetCommandType();
        data[1] = (byte) this.Response;
        AesKey.CopyTo(data, 2);
        return data;
    }


    public override CommandType GetCommandType()
    {
        return CommandType.COMM_GET_AES_KEY;
    }

    public byte[] GetAESKey()
    {
        return AesKey;
    }
}