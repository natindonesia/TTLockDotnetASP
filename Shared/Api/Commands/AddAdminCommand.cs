using System.Text;
using Shared.Entity;
using Shared.Enums;

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
        var adminUnlock = new byte[8];

        // Write adminPs as a big-endian int at position 0
        var adminPsBytes = BitConverter.GetBytes(AdminPassword);
        if (BitConverter.IsLittleEndian) Array.Reverse(adminPsBytes);
        Array.Copy(adminPsBytes, 0, adminUnlock, 0, 4);

        // Write unlockKey as a big-endian int at position 4
        var unlockKeyBytes = BitConverter.GetBytes(UnlockKey);
        if (BitConverter.IsLittleEndian) Array.Reverse(unlockKeyBytes);
        Array.Copy(unlockKeyBytes, 0, adminUnlock, 4, 4);

        // Convert the string "SCIENER" to bytes
        var scienerBytes = Encoding.ASCII.GetBytes("SCIENER");

        // Concatenate the byte arrays
        var result = new byte[adminUnlock.Length + scienerBytes.Length];
        Buffer.BlockCopy(adminUnlock, 0, result, 0, adminUnlock.Length);
        Buffer.BlockCopy(scienerBytes, 0, result, adminUnlock.Length, scienerBytes.Length);

        return result;
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