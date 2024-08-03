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

    public static async Task Init(TTDevice device)
    {
        var request = Command.From(device, new InitCommand());
        await device.SendCommandAndWait(request);
    }
}