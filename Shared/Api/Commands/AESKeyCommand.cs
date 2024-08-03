using System.Text;
using Shared.Enums;

namespace Shared.Api.Commands;

public class AESKeyCommand : AbstractCommand
{
    protected byte[]? AesKey;

    public override void ProcessData()
    {
    }

    public void SetAESKey(byte[] aesKey)
    {
        AesKey = aesKey;
    }

    public override byte[] Build()
    {
        if (AesKey == null)
        {
            return Encoding.UTF8.GetBytes("SCIENER");
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
}