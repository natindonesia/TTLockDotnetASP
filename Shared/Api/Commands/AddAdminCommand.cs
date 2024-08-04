using System.Text;
using Shared.Entity;
using Shared.Enums;
using Shared.Utils;

namespace Shared.Api.Commands;

public class AddAdminCommand : AbstractCommand
{
    public AddAdminCommand(int adminPassword, int unlockKey) : base()
    {
        AdminPassword = adminPassword;
        UnlockKey = unlockKey;
    }

    public AddAdminCommand(byte[]? data) : base(data)
    {
    }

    public int AdminPassword { get; set; }
    public int UnlockKey { get; set; }

    public override void ProcessData()
    {
        var str = Encoding.UTF8.GetString(Data);
        if (str != "SCIENER") Response = CommandResponse.FAILED;
    }

    public override byte[] Build()
    {
        var values = new byte[4 + 4 + 7]; //4 + 4 + 7

        var adminPsBytes = DigitUtil.IntegerToByteArray(AdminPassword);
        var unlockKeyBytes = DigitUtil.IntegerToByteArray(UnlockKey);
        var scienerBytes = Encoding.UTF8.GetBytes("SCIENER");

        Buffer.BlockCopy(adminPsBytes, 0, values, 0, 4);
        Buffer.BlockCopy(unlockKeyBytes, 0, values, 4, 4);
        Buffer.BlockCopy(scienerBytes, 0, values, 8, 7);

        return values;
    }

    public override CommandType GetCommandType()
    {
        return CommandType.COMM_ADD_ADMIN;
    }

    public Admin GetAdminData()
    {
        return new Admin
        {
            AdminPs = AdminPassword,
            UnlockKey = UnlockKey
        };
    }
}