using System.Globalization;
using Shared.Api.Commands;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Utils;

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
        await device.SendCommandAndWait(request);
    }

    public static async Task<byte[]> GetAesKey(TTDevice device)
    {
        var command = new GetAesKeyCommand();
        var request = Command.From(device, command);
        var res = await device.SendCommandAndWait(request);
        var data = res.GetData(device.GetAesKeyArray());
        var resCommand = new GetAesKeyCommand(data);
        if (resCommand.Response != CommandResponse.SUCCESS)
        {
            throw new Exception("GetAesKey failed");
        }

        resCommand.ProcessData();
        return resCommand.GetAESKey();
    }

    public static async Task CalibrationTime(TTDevice device)
    {
        var command = new CalibrationTimeCommand(DateTime.Now);
        var request = Command.From(device, command);
        var res = await device.SendCommandAndWait(request);
        var resCommand = new CalibrationTimeCommand(res.GetData(device.GetAesKeyArray()));
        if (resCommand.Response != CommandResponse.SUCCESS)
        {
            throw new BaseException("CalibrationTime failed");
        }
    }

    public static async Task<DeviceFeaturesCommand> DeviceFeatures(TTDevice device)
    {
        var command = new DeviceFeaturesCommand();
        var request = Command.From(device, command);
        var res = await device.SendCommandAndWait(request);
        var resCommand = new DeviceFeaturesCommand(res.GetData(device.GetAesKeyArray()));
        if (resCommand.Response != CommandResponse.SUCCESS)
        {
            throw new BaseException("DeviceFeatures failed");
        }

        resCommand.ProcessData();
        return resCommand;
    }

    public static async Task<AutoLockManageCommand> AutoLockManage(TTDevice device, ushort? seconds)
    {
        var command = new AutoLockManageCommand();
        if (seconds.HasValue)
        {
            command.SetTime(seconds.Value);
        }

        var request = Command.From(device, command);
        var res = await device.SendCommandAndWait(request);
        var resCommand = new AutoLockManageCommand(res.GetData(device.GetAesKeyArray()));
        if (resCommand.Response != CommandResponse.SUCCESS)
        {
            throw new BaseException("AutoLockManage failed");
        }

        resCommand.ProcessData();
        return resCommand;
    }

    public static async Task<string> SetAdminKeyboardPwdCommand(TTDevice device, string? password = null)
    {
        if (password == null)
        {
            password = "";
            for (var i = 0; i < 7; i++)
            {
                password += (Math.Floor(Random.NextDouble() * 10)).ToString(CultureInfo.CurrentCulture);
            }
        }

        var command = new SetAdminKeyboardPwdCommand(password);
        var request = Command.From(device, command);
        var res = await device.SendCommandAndWait(request);
        var resCommand = new SetAdminKeyboardPwdCommand(res.GetData(device.GetAesKeyArray()));
        if (resCommand.Response != CommandResponse.SUCCESS)
        {
            throw new BaseException("SetAdminKeyboardPwdCommand failed");
        }

        return password;
    }

    public static async Task<uint> CheckUserTime(TTDevice device, int uid = 0, long startDate = 949338000000,
        long endDate = 4099741200000, uint lockFlagPos = 0)
    {
        var command = new CheckUserTimeCommand(uid, startDate, endDate, lockFlagPos);
        var request = Command.From(device, command);
        var res = await device.SendCommandAndWait(request);
        var data = res.GetData(device.GetAesKeyArray());
        var resCommand = new CheckUserTimeCommand(data);
        if (resCommand.Response != CommandResponse.SUCCESS)
        {
            throw new BaseException("CheckUserTime failed");
        }

        resCommand.ProcessData();

        return resCommand.GetPsFromLock();
    }

    public static async Task<UnlockCommand> Unlock(TTDevice de, uint psFromLock)
    {
        var command = new UnlockCommand();
        command.SetSum(psFromLock, de.LockData.PrivateData.Admin.UnlockKey);
        var request = Command.From(de, command);
        var res = await de.SendCommandAndWait(request);
        var resCommand = new UnlockCommand(res.GetData(de.GetAesKeyArray()));
        if (resCommand.Response != CommandResponse.SUCCESS)
        {
            throw new BaseException("Unlock failed");
        }

        resCommand.ProcessData();
        return resCommand;
    }

    public static async Task<AddAdminCommand> AddAdmin(TTDevice device)
    {
        var adminPassword = (int) Math.Floor(Random.NextDouble() * 100000000);
        var unlockKey = (int) Math.Floor(Random.NextDouble() * 1000000000);
        System.Console.WriteLine($"AdminPassword: {adminPassword}, UnlockKey: {unlockKey}");
        var command = new AddAdminCommand(adminPassword, unlockKey);
        var request = Command.From(device, command);
        var res = await device.SendCommandAndWait(request);
        var resCommand = new AddAdminCommand(res.GetData(device.GetAesKeyArray()));
        resCommand.ProcessData();
        if (resCommand.Response != CommandResponse.SUCCESS)
        {
            throw new BaseException("AddAdmin failed");
        }

        return command;
    }

    public static async Task OperateFinished(TTDevice ttDevice)
    {
        var command = new OperateFinishedCommand();
        var request = Command.From(ttDevice, command);
        var res = await ttDevice.SendCommandAndWait(request);
        var resCommand = new OperateFinishedCommand(res.GetData(ttDevice.GetAesKeyArray()));
        if (resCommand.Response != CommandResponse.SUCCESS)
        {
            throw new Exception("OperateFinished failed");
        }
    }
}