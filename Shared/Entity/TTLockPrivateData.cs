namespace Shared.Entity;

public record TTLockPrivateData
{
    public byte[] AesKey { get; set; } = new byte[0];
    public Admin Admin { get; set; }
    public string AdminPasscode { get; set; } = "";
    public CodeSecret[] PwdInfo { get; set; } = new CodeSecret[0];
}