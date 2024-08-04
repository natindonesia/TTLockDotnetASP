using System.Collections.Concurrent;
using Shared.Api;
using Shared.Entity;
using Shared.Enums;

namespace Shared;

public class TTDevice : IDisposable
{
    public const sbyte GAP_ADTYPE_LOCAL_NAME_COMPLETE = 9; //!< Complete local name
    public const sbyte GAP_ADTYPE_POWER_LEVEL = 10; //!< TX Power Level: 0xXX: -127 to +127 dBm

    public const sbyte GAP_ADTYPE_MANUFACTURER_SPECIFIC = -1;

    /**
     * 闭锁
     */
    public const sbyte STATUS_PARK_LOCK = 0;

    /**
     * 开锁无车
     */
    public const sbyte STATUS_PARK_UNLOCK_NO_CAR = 1;

    /**
     * 状态未知
     */
    public const sbyte STATUS_PARK_UNKNOWN = 2;

    /**
     * 开锁有车
     */
    public const sbyte STATUS_PARK_UNLOCK_HAS_CAR = 3;

    public static readonly byte[] CRLF = new byte[] {0x0D, 0x0A};
    public static readonly String UuidService = "00001910-0000-1000-8000-00805f9b34fb";
    public static readonly String UuidWrite = "0000fff2-0000-1000-8000-00805f9b34fb";
    public static readonly String UuidRead = "0000fff4-0000-1000-8000-00805f9b34fb";


    private static readonly String DeviceInformationService = "0000180a-0000-1000-8000-00805f9b34fb";
    private static readonly String ReadModelNumberUuid = "00002a24-0000-1000-8000-00805f9b34fb";
    private static readonly String ReadFirmwareRevisionUuid = "00002a26-0000-1000-8000-00805f9b34fb";
    private static readonly String ReadHardwareRevisionUuid = "00002a27-0000-1000-8000-00805f9b34fb";
    private static readonly String ReadManufacturerNameUuid = "00002a29-0000-1000-8000-00805f9b34fb";

    // originally from Java where Byte is -128 to 127 and C# Byte is 0 to 255
    protected readonly sbyte[] ScanRecord;
    public string Address = "";
    protected sbyte BatteryCapacity = 0;
    protected long Date = 0;

    protected IBluetoothDevice Device;
    protected int DisconnectStatus = 0;
    protected string Firmware = "unknown";
    protected byte GroupId = 0;
    protected string Hardware = "unknown";
    protected bool HasEvents = true;
    protected string Id = "";

    protected byte[] IncomingData = [];

    protected volatile bool IsConnected = false;

    protected bool IsDfuMode = false;
    protected bool IsNoLockService = false;
    protected bool IsSettingMode = false;
    protected bool IsTouch = false;
    protected bool IsUnlock = false;

    protected bool IsWristband,
        IsRoomLock,
        IsSafeLock,
        IsBicycleLock,
        IsLockCar,
        IsGlassLock,
        IsPadlock,
        IsCylinder,
        IsLift,
        IsPowerSaver,
        IsRemoteControlDevice = false;

    public TTLockData LockData = new TTLockData();

    public LockType LockType = LockType.UNKNOWN;
    protected string Manufacturer = "unknown";
    protected string Model = "unknown";
    public string? Name = "unknown";
    protected byte OrgId = 0;
    protected int ParkStatus = 0;
    public sbyte ProtocolType = 0;
    public sbyte ProtocolVersion = 0;
    protected int RemoteUnlockSwitch = 0;

    protected ConcurrentQueue<Command> ResponseQueue = new ConcurrentQueue<Command>();
    protected string Rssi = "0";
    public sbyte Scene = 0;
    protected sbyte TxPowerLevel = 0;
    protected string Uuid = "";

    public TTDevice(IBluetoothDevice device)
    {
        ScanRecord = new sbyte[device.RawData.Length];
        for (var i = 0; i < device.RawData.Length; i++) ScanRecord[i] = (sbyte) device.RawData[i];
        Device = device;
        Initialize();
    }


    public bool IsTelinkGatewayDfuMode { get; set; }

    public GatewayType GatewayType { get; set; }

    public bool IsInitialized => LockData.PrivateData.AesKey.Length != 0 || !IsSettingMode;

    public void Dispose()
    {
        Device.Dispose();
    }

    /**
     *
Signed Byte
[2, 1, 6, 2, 10, -65, 3, 2, 16, 25, 18, -1, 5, 3, 2, 28, 100, -80, 0, -12, -7, 83, 101, -3, -82, 76, -83, -63, -14, 11, 9, 68, 48, 49, 95, 102, 100, 97, 101, 52, 99, 5, 18, 20, 0, 36, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]


1st Set:

02: Length: 2 Bytes
01: Type: Flags
06: Flag - 02 && 04: LE General Discoverable && BR/EDR Not Supported
2nd Set:

10: Length: 16 bytes
FF: Type: Manufacture Data
107803E80000000000006400232900: Data specific to the manufacturer
3rd Set:

09: Length: 9 bytes
09: Type: Complete Local Name
4536372045414145: E67 EAAE (Name of device in ASCII)
     */
    public void Initialize()
    {
        var scanRecord = ScanRecord;
        var scanRecordLength = scanRecord.Length;
        var index = 0;
        var nameIsScienerDfu = false;
        var isHasMAC = false;

        while (index < scanRecordLength)
        {
            int len = scanRecord[index];
            if (len == 0) break;
            var adtype = scanRecord[index + 1];
            switch (adtype)
            {
                case GAP_ADTYPE_LOCAL_NAME_COMPLETE:
                    var nameBytes = new byte[len - 1];
                    Array.Copy(scanRecord, index + 2, nameBytes, 0, len - 1);
                    var name = System.Text.Encoding.UTF8.GetString(nameBytes);
                    if (string.IsNullOrEmpty(Name)) Name = name;

                    if (name.Equals("ScienerDfu")) nameIsScienerDfu = true;

                    if (Name.ToUpper().StartsWith("LOCK_")) IsRoomLock = true;
                    break;
                case GAP_ADTYPE_MANUFACTURER_SPECIFIC:
                    GatewayType = GatewayTypeMethods.GetGatewayType(scanRecord.Skip(index + 2).Take(2).ToArray());

                    isHasMAC = true;
                    var offset = 2;
                    ProtocolType = scanRecord[index + offset++];
                    ProtocolVersion = scanRecord[index + offset++];

                    if (ProtocolType == 0x12 && ProtocolVersion == 0x19) // Plug in DFU mode
                    {
                        IsDfuMode = true;
                        return;
                    }

                    if (ProtocolType == 0x13 && ProtocolVersion == 0x19) // Gateway DFU mode (Telink chip)
                    {
                        IsTelinkGatewayDfuMode = true;
                        return;
                    }

                    if (ProtocolType == 0xff && ProtocolVersion == 0xff)
                    {
                        IsDfuMode = true;
                        return;
                    }

                    if (ProtocolType == 0x34 && ProtocolVersion == 0x12) IsWristband = true;
                    //if (BluetoothImpl.ScanBongOnly) // Wristband only return;
                    if (ProtocolType == 0x05 && ProtocolVersion == 0x03) // Third generation lock
                    {
                        Scene = scanRecord[index + offset++];
                    }
                    else // Other locks
                    {
                        offset = 6; // Other protocols start from the 6th byte
                        ProtocolType = scanRecord[index + offset++];
                        ProtocolVersion = scanRecord[index + offset];
                        offset = 9; // Scene offset
                        Scene = scanRecord[index + offset++];
                    }

                    if (ProtocolType < 0x05 ||
                        GetLockType() == LockType.LOCK_TYPE_V2S) // Old locks without broadcast
                    {
                        IsRoomLock = true;
                        return;
                    }

                    if (Scene <= 3)
                        IsRoomLock = true;
                    else
                        ProcessScene();

                    IsUnlock = (scanRecord[index + offset] & 0x01) == 1;
                    IsSettingMode = (scanRecord[index + offset] & 0x04) != 0;
                    if (GetLockType() == LockType.LOCK_TYPE_V3 || GetLockType() == LockType.LOCK_TYPE_V3_CAR)
                    {
                        IsTouch = (scanRecord[index + offset] & 0x08) != 0; // Third generation lock touch flag
                    }
                    else if (GetLockType() == LockType.LOCK_TYPE_CAR) // Second generation parking lock
                    {
                        IsTouch = false; // Default to false for parking locks
                        IsLockCar = true;
                    }

                    if (IsLockCar) // Combined status of bits 0 and 4
                    {
                        if (IsUnlock)
                        {
                            if ((scanRecord[index + offset] & 0x10) == 1)
                                ParkStatus = STATUS_PARK_UNLOCK_HAS_CAR;
                            else
                                ParkStatus = STATUS_PARK_UNKNOWN;
                        }
                        else
                        {
                            if ((scanRecord[index + offset] & 0x10) == 1)
                                ParkStatus = STATUS_PARK_UNLOCK_NO_CAR;
                            else
                                ParkStatus = STATUS_PARK_LOCK;
                        }
                    }

                    // Battery capacity offset
                    offset++;
                    BatteryCapacity = scanRecord[index + offset];
                    // MAC address offset
                    offset += 3;
                    if (string.IsNullOrEmpty(Address))
                    {
                        var macAddressBytes = scanRecord.Skip(index + offset).Take(6).ToArray();
                        // convert to bytes
                        var macAddress = new byte[6];
                        for (var i = 0; i < 6; i++) macAddress[i] = (byte) macAddressBytes[i];
                        //Address = BitConverter.ToString(macAddress).Replace("-", ":");
                    }

                    break;
                case GAP_ADTYPE_POWER_LEVEL:
                    TxPowerLevel = scanRecord[index + 2];
                    break;
                default:
                    break;
            }

            index += len + 1;
        }

        if (nameIsScienerDfu && !isHasMAC) IsDfuMode = true;
    }

    private void ProcessScene()
    {
        switch (Scene)
        {
            case Entity.Scene.GLASS_LOCK: //门禁
                IsGlassLock = true;
                break;
            case Entity.Scene.SAFE_LOCK: //保险箱锁
            case Entity.Scene.SAFE_LOCK_SINGLE_PASSCODE:
                IsSafeLock = true;
                break;
            case Entity.Scene.BICYCLE_LOCK: //自行车锁
                IsBicycleLock = true;
                break;
            case Entity.Scene.PARKING_LOCK: //车位锁
                IsLockCar = true;
                break;
            case Entity.Scene.PAD_LOCK: //挂锁
                IsPadlock = true;
                break;
            case Entity.Scene.CYLINDER: //锁芯
                IsCylinder = true;
                break;
            case Entity.Scene.REMOTE_CONTROL_DEVICE:
                if (ProtocolType == 0x05 && ProtocolVersion == 0x03) //二代车位锁场景也是10 增加一个三代锁的判断
                    IsRemoteControlDevice = true;
                break;
            case Entity.Scene.LIFT:
                IsLift = true;
                break;
            case Entity.Scene.POWER_SAVER:
                IsPowerSaver = true;
                break;
            default:
                break;
        }
    }

    public static TTDevice? FromBluetoothDevice(IBluetoothDevice device)
    {
        try
        {
            return new TTDevice(device);
        }
        catch (Exception ex) when (
            ex is ArgumentException ||
            ex is IndexOutOfRangeException
        )
        {
            return null;
        }
    }

    //TODO: Second generation
    ////Connecting directly based on mac address does not have scanning information. The data here is inaccurate
    public LockType GetLockType()
    {
        if (ProtocolType == 0x05 && ProtocolVersion == 0x03 && Scene == 0x07)
            LockType = LockType.LOCK_TYPE_V3_CAR;
        else if (ProtocolType == 0x0a && ProtocolVersion == 0x01)
            LockType = LockType.LOCK_TYPE_CAR;
        else if (ProtocolType == 0x0b && ProtocolVersion == 0x01)
            LockType = LockType.LOCK_TYPE_MOBI;
        else if (ProtocolType == 0x05 && ProtocolVersion == 0x04)
            LockType = LockType.LOCK_TYPE_V2S_PLUS;
        else if (ProtocolType == 0x05 && ProtocolVersion == 0x03)
            LockType = LockType.LOCK_TYPE_V3;
        else if ((ProtocolType == 0x05 && ProtocolVersion == 0x01) ||
                 (Name != null && Name.ToUpper().StartsWith("LOCK_"))) LockType = LockType.LOCK_TYPE_V2S;
        return LockType;
    }

    protected void OnIncomingData(byte[] data)
    {
        try
        {
            IncomingData = IncomingData.Concat(data).ToArray();
            if (IncomingData.Length < 2) return;
            // get last 2 bytes
            var ending = IncomingData.Skip(IncomingData.Length - 2).ToArray();
            if (!ending.SequenceEqual(CRLF)) return;
            // format in hex
            var hex = BitConverter.ToString(IncomingData).Replace("-", " ");
            Console.WriteLine($"Incoming data: ({hex})");

            // remove CRLF
            IncomingData = IncomingData.Take(IncomingData.Length - 2).ToArray();

            var command = new Command(IncomingData);
            ResponseQueue.Enqueue(command);


            IncomingData = [];
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to process incoming data");
            Console.WriteLine(ex);
        }
    }

    public async Task Connect()
    {
        if (IsConnected) return;
        await Device.OnDisconnected(() => IsConnected = false);
        await ReadBasicInfo();

        await Device.SubscribeCharacteristic(UuidService, UuidRead, OnIncomingData);
        IsConnected = true;
    }

    public async Task ReadBasicInfo()
    {
        Name = await Device.GetName();
        if (Name != null && await Device.HasService("00001800-0000-1000-8000-00805f9b34fb"))
        {
            var data = await Device.ReadCharacteristic("00001800-0000-1000-8000-00805f9b34fb",
                "0000a2a00-0000-1000-8000-00805f9b34fb");
            Name = System.Text.Encoding.UTF8.GetString(data);
        }

        if (await Device.HasService(DeviceInformationService))
        {
            var data = await Device.ReadCharacteristic(DeviceInformationService, ReadModelNumberUuid);
            Model = System.Text.Encoding.UTF8.GetString(data);
            data = await Device.ReadCharacteristic(DeviceInformationService, ReadFirmwareRevisionUuid);
            Firmware = System.Text.Encoding.UTF8.GetString(data);
            data = await Device.ReadCharacteristic(DeviceInformationService, ReadHardwareRevisionUuid);
            Hardware = System.Text.Encoding.UTF8.GetString(data);
            data = await Device.ReadCharacteristic(DeviceInformationService, ReadManufacturerNameUuid);
            Manufacturer = System.Text.Encoding.UTF8.GetString(data);
            LockData.Address = Address;
        }
    }

    public async Task SendCommand(Command command, bool ignoreCrc = false)
    {
        await Connect();
        var data = command.BuildCommand();
        // add CRLF to the end
        data = data.Concat(CRLF).ToArray();
#if DEBUG
        var hex = BitConverter.ToString(data).Replace("-", " ");
        Console.WriteLine($"Sending command: {command.GetCommandType()} ({hex})");

#endif
        await Device.WriteCharacteristic(UuidService, UuidWrite, data);
    }


    public async Task<Command> SendCommandAndWait(Command command, bool ignoreCrc = false)
    {
        await SendCommand(command, ignoreCrc);
        var timeout = 5000;
        var start = DateTime.Now;
        while (DateTime.Now - start < TimeSpan.FromMilliseconds(timeout))
        {
            if (ResponseQueue.TryDequeue(out var response))
            {
                if (!response.isChecksumValid())
                {
                    Console.WriteLine("Checksum is invalid, retrying?");
                    continue;
                }

                return response;
            }

            await Task.Delay(100);
        }

        throw new TimeoutException("Command timed out waiting for response");
    }


    public override string ToString()
    {
        return $"TTDevice: {Name} ({Address}) - {LockType}";
    }

    public async Task InitLock()
    {
        Console.WriteLine("Init lock");
        await TTLockAPI.Init(this);
        var aes = await TTLockAPI.GetAesKey(this);
        LockData.PrivateData.AesKey = aes;
        var admin = await TTLockAPI.AddAdmin(this);
        LockData.PrivateData.Admin = admin.GetAdminData();
        this.IsSettingMode = false;
    }

    public byte[] GetAesKeyArray()
    {
        return this.LockData.PrivateData.AesKey.Length != 0
            ? this.LockData.PrivateData.AesKey
            : TTLockAPI.DefaultAesKeyArray;
    }


    public async Task Unlock()
    {
        var psFromLock = await TTLockAPI.CheckUserTime(this);
        var unlockCommand = await TTLockAPI.Unlock(this, psFromLock);
    }
}