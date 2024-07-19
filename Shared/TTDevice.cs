using Shared.Enums;

namespace Shared;

public class TTDevice : IDisposable
{

    protected string Id = "";
    protected string Uuid = "";
    public string Name = "unknown";
    protected string Manufacturer = "unknown";
    protected string Model = "unknown";
    protected string Hardware = "unknown";
    protected string Firmware = "unknown";
    protected string Address = "";
    protected string Rssi = "0";
    public sbyte ProtocolType = 0;
    public sbyte ProtocolVersion = 0;
    public byte Scene = 0;
    protected byte GroupId = 0;
    protected byte OrgId = 0;
    public LockType LockType = LockType.UNKNOWN;
    protected bool IsTouch = false;
    protected bool IsUnlock = false;
    protected bool HasEvents = true;
    protected bool IsSettingMode = false;
    protected byte TxPowerLevel = 0;
    protected byte BatteryCapacity = 0;
    protected long Date = 0;

    protected bool IsWristband,
        IsRoomLock,
        IsSafeLock,
        IsBicycleLock,
        IsLockCar,
        IsGlassLock,
        IsPadlock,
        IsCylinder,
        IsRemoteControlDevice = false;

    protected bool IsDfuMode = false;
    protected bool IsNoLockService = false;
    protected int RemoteUnlockSwitch = 0;
    protected int DisconnectStatus = 0;
    protected int ParkStatus = 0;
    public static  byte GAP_ADTYPE_LOCAL_NAME_COMPLETE = 0X09;	//!< Complete local name
    public static  byte GAP_ADTYPE_POWER_LEVEL = 0X0A;	//!< TX Power Level: 0xXX: -127 to +127 dBm
    public static  byte GAP_ADTYPE_MANUFACTURER_SPECIFIC = (byte) 0XFF; //!< Manufacturer Specific Data: first 2 octets contain the Company Inentifier Code followed by the additional manufacturer specific data

    protected BluetoothDevice Device;
    protected readonly byte[] ManufacturerData;

    public TTDevice(byte[] manufacturerData, BluetoothDevice device)
    {
        ManufacturerData = manufacturerData;
        Device = device;
        Initialize();
    }

    public void Initialize()
    {
        var manufacturerData = ManufacturerData;
           // must at least 15 bytes in length
        if (manufacturerData.Length < 15)
        {
            throw new ArgumentException("Manufacturer data must be at least 15 bytes in length");
        }
        int offset = 0;
        this.ProtocolType = (sbyte) manufacturerData[offset++];
        this.ProtocolVersion = (sbyte) manufacturerData[offset++];
        if (this.ProtocolType == 18 && this.ProtocolVersion == 25)
        {
            this.IsDfuMode = true;
            return;
        }
        if (this.ProtocolType == -1 && this.ProtocolVersion == -1)
        {
            this.IsDfuMode = true;
            return;
        }
        if (this.ProtocolType == 52 && this.ProtocolVersion == 18)
        {
            this.IsWristband = true;
        }
        if (this.ProtocolType == 5 && this.ProtocolVersion == 3)
        {
            this.Scene = manufacturerData[offset++];
        }
        else
        {
            offset = 4;
            this.ProtocolType = (sbyte) manufacturerData[offset++];
            this.ProtocolVersion = (sbyte) manufacturerData[offset++];
            offset = 7;
            this.Scene = manufacturerData[offset++];
        }
        if (this.ProtocolType < 5 || LockVersion.GetLockType(this) == LockType.LOCK_TYPE_V2S)
        {
            this.IsRoomLock = true;
        }
        if (this.Scene <= 3)
        {
            this.IsRoomLock = true;
        }
        else
        {
            switch (this.Scene)
            {
                case 4:
                    this.IsGlassLock = true;
                    break;
                case 5:
                case 11:
                    this.IsSafeLock = true;
                    break;
                case 6:
                    this.IsBicycleLock = true;
                    break;
                case 7:
                    this.IsLockCar = true;
                    break;
                case 8:
                    this.IsPadlock = true;
                    break;
                case 9:
                    this.IsCylinder = true;
                    break;
                case 10:
                    if (this.ProtocolType == 5 && this.ProtocolVersion == 3)
                    {
                        this.IsRemoteControlDevice = true;
                    }
                    break;
            }
        }

        byte paramsData = (byte)manufacturerData[offset];

        this.IsUnlock = ((paramsData & 0x1) == 0x1);

        this.HasEvents = ((paramsData & 0x2) == 0x2);

        this.IsSettingMode = ((paramsData & 0x4) != 0x0);
        if (LockVersion.GetLockType(this) == LockType.LOCK_TYPE_V3 || LockVersion.GetLockType(this) == LockType.LOCK_TYPE_V3_CAR)
        {
            this.IsTouch = ((paramsData & 0x8) != 0x0);
        }
        else if (LockVersion.GetLockType(this) == LockType.LOCK_TYPE_CAR)
        {
            this.IsTouch = false;
            this.IsLockCar = true;
        }
        if (this.IsLockCar)
        {
            if (this.IsUnlock)
            {
                if ((paramsData & 0x10) == 0x10)
                {
                    this.ParkStatus = 3;
                }
                else
                {
                    this.ParkStatus = 2;
                }
            }
            else if ((paramsData & 0x10) == 0x10)
            {
                this.ParkStatus = 1;
            }
            else
            {
                this.ParkStatus = 0;
            }
        }
        offset++;

        this.BatteryCapacity = manufacturerData[offset];

        // offset += 3 + 4; // Offset in original SDK is + 3, but in scans it's actually +4
        offset = manufacturerData.Length - 6; // let's just get the last 6 bytes
        byte[] macBuf = new byte[6];
        Array.Copy(manufacturerData, offset, macBuf, 0, 6);
        Array.Reverse(macBuf);
        this.Address = BitConverter.ToString(macBuf).Replace("-", ":").ToUpper();
    }

public static TTDevice? FromBluetoothDevice(BluetoothDevice device)
    {
        try
        {
            return new TTDevice(device.ManufacturerData, device);
        }
        catch (ArgumentException ex)
        {
            return null;
        }
    }

    protected byte[] IncomingData = [];
    public static readonly byte[] CRLF = new byte[] { 0x0D, 0x0A };

    protected void OnIncomingData(byte[] data)
    {
        IncomingData = IncomingData.Concat(data).ToArray();
        if (IncomingData.Length >= 2)
        {
            // get last 2 bytes
            var ending = IncomingData.Skip(IncomingData.Length - 2).ToArray();
            if (ending.SequenceEqual(CRLF))
            {
                // we have a complete message
                var message = System.Text.Encoding.UTF8.GetString(IncomingData);
                Console.WriteLine($"Received: {message}");
                IncomingData = [];
            }
        }
    }

    protected bool IsConnected = false;
    public async Task Connect()
    {
        if (IsConnected)
        {
            return;
        }
        await ReadBasicInfo();

        await Device.SubscribeCharacteristic("1910", "FFF4", OnIncomingData);
        IsConnected = true;
    }

    public async Task ReadBasicInfo()
    {
        if (await Device.HasService("1800"))
        {
            byte[] data = await Device.ReadCharacteristic("1800", "2A00");
            Name = System.Text.Encoding.UTF8.GetString(data);
        }

        if (await Device.HasService("180A"))
        {
            byte[] data = await Device.ReadCharacteristic("180A", "2A24");
            Model = System.Text.Encoding.UTF8.GetString(data);
            data = await Device.ReadCharacteristic("180A", "2A26");
            Firmware = System.Text.Encoding.UTF8.GetString(data);
            data = await Device.ReadCharacteristic("180A", "2A27");
            Hardware = System.Text.Encoding.UTF8.GetString(data);
            data = await Device.ReadCharacteristic("180A", "2A29");
            Manufacturer = System.Text.Encoding.UTF8.GetString(data);
        }
    }

    public override string ToString()
    {
        return $"TTDevice: {Name} ({Address}) - {LockType}";
    }

    public void Dispose()
    {
        Device.Dispose();

    }
}
