using Shared.Enums;

namespace Shared.Entity;

public record TTLockData
{
    // MAC address
    public string Address { get; set; } = "";

    // Battery level
    public float Battery { get; set; } = 0;

    // Signal
    public int Rssi { get; set; } = 0;

    // Auto lock time in seconds
    public int AutoLockTime { get; set; } = 0;

    // -1 unknown, 0 locked, 1 unlocked
    public int LockedStatus { get; set; } = -1;

    // Lock private data
    public TTLockPrivateData PrivateData { get; set; } = new TTLockPrivateData();

    // Capabilities
    public HashSet<FeatureValue> Features { get; set; } = new HashSet<FeatureValue>();
    //  Operation Log entries
}