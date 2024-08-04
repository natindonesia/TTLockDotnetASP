using Shared.Api.Commands;
using Shared.Enums;

namespace Shared.Api;

public static class TTLockAPI
{
    public static readonly byte[] DefaultAesKeyArray =
    {
        (byte) 0x98, (byte) 0x76, (byte) 0x23, (byte) 0xE8,
        (byte) 0xA9, (byte) 0x23, (byte) 0xA1, (byte) 0xBB,
        (byte) 0x3D, (byte) 0x9E, (byte) 0x7D, (byte) 0x03,
        (byte) 0x78, (byte) 0x12, (byte) 0x45, (byte) 0x88
    };

    public static readonly Random Random = new Random();


    public static async Task Init(TTDevice device)
    {
        var request = Command.From(device, new InitCommand());
        var res = await device.SendCommandAndWait(request);
        res.Validate();
    }

    public static async Task<byte[]> GetAesKey(TTDevice device)
    {
        var command = new GetAesKeyCommand();
        var request = Command.From(device, command);
        var res = await device.SendCommandAndWait(request);
        var data = res.GetData(device.GetAesKeyArray());
        var resCommand = new GetAesKeyCommand(data);
        resCommand.ProcessData();
        return resCommand.GetAESKey();
    }

    public static async Task<int> CheckUserTime(TTDevice device, uint uid = 0, string startDate = "000101000000",
        string endDate = "991231235959", uint lockFlagPos = 0)
    {
        var command = new CheckUserTimeCommand();
        command.SetPayload(uid, startDate, endDate, lockFlagPos);
        var request = Command.From(device, command);
        var res = await device.SendCommandAndWait(request);
        var data = res.GetData(device.GetAesKeyArray());
        var resCommand = new CheckUserTimeCommand(data);
        resCommand.ProcessData();
        return resCommand.GetPsFromLock();
    }

    public static async Task<UnlockCommand> Unlock(TTDevice de, int psFromLock)
    {
        var command = new UnlockCommand();
        command.SetSum(psFromLock, de.LockData.PrivateData.Admin.UnlockKey);
        var request = Command.From(de, command);
        var res = await de.SendCommandAndWait(request);
        var resCommand = new UnlockCommand(res.GetData(de.GetAesKeyArray()));
        resCommand.ProcessData();
        return resCommand;
    }

    public static async Task<AddAdminCommand> AddAdmin(TTDevice device)
    {
        var adminPassword = (int) (Random.NextDouble() * 100000000);
        var unlockKey = (int) (Random.NextDouble() * 100000000);
        var command = new AddAdminCommand(adminPassword, unlockKey);
        var request = Command.From(device, command);
        await device.SendCommandAndWait(request);

        return command;
    }
}