using System.Text.Json;
using Shared.Api.Commands;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Utils;

namespace Shared.Api;

/**
 * Created by Smartlock on 2016/5/27.
 */
public class Command
{
    private static bool DBG = false;

    public static readonly byte COMM_INITIALIZATION = (byte) 'E';
    public static readonly byte COMM_GET_AES_KEY = 0x19;
    public static readonly byte COMM_RESPONSE = (byte) 'T';

    /**
     * 添加管理
     */
    public static readonly byte COMM_ADD_ADMIN = (byte) 'V';

    /**
     * 校验管理员
     */
    public static readonly byte COMM_CHECK_ADMIN = (byte) 'A';

    /**
     * 管理员键盘密码
     */
    public static readonly byte COMM_SET_ADMIN_KEYBOARD_PWD = (byte) 'S';

    /**
     * 删除密码
     */
    public static readonly byte COMM_SET_DELETE_PWD = (byte) 'D';

    /**
     * 设置锁名称
     */
    public static readonly byte COMM_SET_LOCK_NAME = (byte) 'N';

    /**
     * 同步键盘密码
     */
    public static readonly byte COMM_SYN_KEYBOARD_PWD = (byte) 'I';

    /**
     * 校验用户时间
     */
    public static readonly byte COMM_CHECK_USER_TIME = (byte) 'U';

    /**
     * 获取车位锁警报记录(动了车位锁)
     * 判断添加以及密码等操作全部完成的指令
     */
    public static readonly byte COMM_GET_ALARM_ERRCORD_OR_OPERATION_FINISHED = (byte) 'W';

    /**
     * 开门
     */
    public static readonly byte COMM_UNLOCK = (byte) 'G';

    /**
     * 关门
     */
    public static readonly byte COMM_LOCK = (byte) 'L';

    /**
     * 校准时间
     */
    public static readonly byte COMM_TIME_CALIBRATE = (byte) 'C';

    /**
     * 管理键盘密码
     */
    public static readonly byte COMM_MANAGE_KEYBOARD_PASSWORD = 0x03;

    /**
     * 获取锁内有效键盘密码
     */
    public static readonly byte COMM_GET_VALID_KEYBOARD_PASSWORD = 0x04;

    /**
     * 获取操作记录
     */
    public static readonly byte COMM_GET_OPERATE_LOG = 0x25;

    /**
     * 随机数验证
     */
    public static readonly byte COMM_CHECK_RANDOM = 0x30;

    /**
     * 三代
     * 密码初始化
     */
    public static readonly byte COMM_INIT_PASSWORDS = 0x31;

    /**
     * 读取密码参数
     */
    public static readonly byte COMM_READ_PWD_PARA = 0x32;

    /**
     * 修改有效键盘密码数量 三代锁
     */
    public static readonly byte COMM_RESET_KEYBOARD_PWD_COUNT = 0x33;

    /**
     * 读取门锁时间
     */
    public static readonly byte COMM_GET_LOCK_TIME = 0x34;

    /**
     * 重置锁
     */
    public static readonly byte COMM_RESET_LOCK = (byte) 'R';

    /**
     * 查询设备特征
     */
    public static readonly byte COMM_SEARCHE_DEVICE_FEATURE = (byte) 0x01;

    /**
     * IC卡管理
     */
    public static readonly byte COMM_IC_MANAGE = 0x05;

    /**
     * 指纹管理
     */
    public static readonly byte COMM_FR_MANAGE = 0x06;

    /**
     * 获取密码列表
     */
    public static readonly byte COMM_PWD_LIST = 0x07;

    /**
     * 设置手环KEY
     */
    public static readonly byte COMM_SET_WRIST_BAND_KEY = 0x35;

    /**
     * 自动闭锁管理(包含门磁)
     */
    public static readonly byte COMM_AUTO_LOCK_MANAGE = 0x36;

    /**
     * 读取设备信息
     */
    public static readonly byte COMM_READ_DEVICE_INFO = (byte) 0x90;

    /**
     * 进入升级模式
     */
    public static readonly byte COMM_ENTER_DFU_MODE = 0x02;

    /**
     * 查询自行车状态(包含门磁)
     */
    public static readonly byte COMM_SEARCH_BICYCLE_STATUS = 0x14;

    /**
     * 闭锁
     */
    public static readonly byte COMM_FUNCTION_LOCK = 0x58;

    /**
     * 屏幕显示密码
     */
    public static readonly byte COMM_SHOW_PASSWORD = 0x59;

    /**
     * 控制远程开锁
     */
    public static readonly byte COMM_CONTROL_REMOTE_UNLOCK = 0x37;

    public static readonly byte COMM_AUDIO_MANAGE = 0x62;

    public static readonly byte COMM_REMOTE_CONTROL_DEVICE_MANAGE = 0x63;

    /**
     * 对于NB联网门锁，通过这条命令，App告诉门锁服务器的地址信息
     */
    public static readonly byte COMM_CONFIGURE_NB_ADDRESS = 0x12;

    /**
     * 酒店锁参数配置
     */
    public static readonly byte COMM_CONFIGURE_HOTEL_DATA = 0x64;

    /**
     * 读取管理员密码
     */
    public static readonly byte COMM_GET_ADMIN_CODE = 0x65;

    /**
     * 常开模式管理
     */
    public static readonly byte COMM_CONFIGURE_PASSAGE_MODE = 0x66;

    /**
     * 开关控制指令(隐私锁、防撬警报、重置锁)
     */
    public static readonly byte COMM_SWITCH = 0x68;

    public static readonly byte COMM_FREEZE_LOCK = 0x61;

    public static readonly byte COMM_LAMP = 0x67;

    /**
     * 死锁指令
     */
    public static readonly byte COMM_DEAD_LOCK = 0x69;

    /**
     * 循环指令
     * 循环时间 添加跟修改都需要清空一下循环时间
     * 如果之前有循环时间 没有做清空操作 之前的循环时间还是会保留下来
     */
    public static readonly byte COMM_CYCLIC_CMD = 0x70;

    /**
     * 无线钥匙管理
     */
    public static readonly byte COMM_KEY_FOB_MANAG = 0x73;

    /**
     * 获取配件电量
     */
    public static readonly byte COMM_ACCESSORY_BATTERY = 0x74;

    /**
     * 无线门磁管理
     */
    public static readonly byte COMM_DOOR_SENSOR_MANAGE = 0x76;

    /**
     * 门打开一定时间未关闭后报警，报警时，有声音提示+日志
     */
    public static readonly byte COMM_DOOR_NOT_CLOSED_WARNING = 0x78;

    /**
     * 3D人脸管理
     */
    public static readonly byte COMM_3D_FACE = 0x79;

    /**
     * 设置接近感应
     * 接近感应设置，3D人脸检测或者二维码检测时，有接近感应功能，当人体或者手机等物体靠近锁时，会自动启动人脸检测或者二维码检测，此功能用于设置开启或者关闭这个感应功能，或者设置感应的距离（远、中、近）
     */
    public static readonly byte COMM_SENSITIVITY_MANAGE = (byte) 0x80;

    /**
     * 掌静脉管理
     */
    public static readonly byte COMM_PALM_VEIN_MANAGE = (byte) 0x83;

    /**
     * 此命令用于控制WiFi锁搜索周围的WiFi路由器的SSID
     */
    public static readonly byte COMM_SCAN_WIFI = (byte) 0xf2;

    /**
     * 此命令用于告诉锁使用哪个WiFi路由器
     */
    public static readonly byte COMM_CONFIG_WIFI_AP = (byte) 0xf3;

    /**
     * 配置服务器地址
     */
    public static readonly byte COMM_CONFIG_SERVER = (byte) 0xf5;

    /**
     * 配置锁本地IP地址
     */
    public static readonly byte COMM_CONFIG_STATIC_IP = (byte) 0xf4;

    /**
     * 读取锁WiFi MAC地址 信号强弱
     */
    public static readonly byte COMM_GET_WIFI_INFO = (byte) 0xf6;

    public static readonly byte COMM_NB_ACTIVATE_CONFIGURATION = 0x13;

    /** ---------------------重置锁命令-------------------------------------------*/
    /**
     * 用默认密钥，获取通讯用的随机数
     */
    public static readonly byte COMM_GET_RAND_CODE = (byte) 0x85;

    /**
     * 不管锁是否有管理员，也不管之前的密钥是什么，用此命令都可以进行重置
     */
    public static readonly byte COMM_RESET_LOCK_BY_CODE = (byte) 0x86;

    /**
     * 贴锁指令
     */
    public static readonly byte COMM_STICKING_LOCK = (byte) 0x8A;

    /**
     * 录入工具通讯指令
     */
    public static readonly byte COMM_VERIFY_LOCK = (byte) 0x8B;

    /**
     * 启动开门方向识别
     */
    public static readonly byte COMM_AUTO_SET_UNLOCK_DIRECTION = (byte) 0x8C;

    public static readonly byte VERSION_LOCK_V1 = 1;
    private static readonly byte APP_COMMAND = (byte) 0xaa; //app命令
    private static readonly int ENCRY_OLD = 1;
    private static readonly int ENCRY_AES_CBC = 2;

    private readonly bool _mIsChecksumValid;
    public readonly byte[] Organization;
    public readonly byte ProtocolType; // 预留	 	1 字节 //reserved 修改为protocol_type
    public readonly byte[] SubOrganization;

    /**
     * 锁类型
     */
    private LockType _lockType;

    private byte _scene;

    // V4版本新加
    private byte _subVersion;
    public byte Checksum; // 校验		1 字节
    public byte CommandType; // 命令字 	1 字节
    protected byte[]? Data; // 数据
    public byte Encrypt; // 加密字		1 字节

    public byte[] Header; // 帧首 		2 字节
    public byte Length; // 长度		1 字节

    public Command(LockVersion lockVersion)
    {
        Header = new byte[2];
        Header[0] = 0x7F;
        Header[1] = 0x5A;
        ProtocolType = (byte) lockVersion.getProtocolType();
        _subVersion = (byte) lockVersion.getProtocolVersion();
        _scene = (byte) lockVersion.getScene();
        Organization = DigitUtil.shortToByteArray(lockVersion.getGroupId());
        SubOrganization = DigitUtil.shortToByteArray(lockVersion.getOrgId());
        Encrypt = APP_COMMAND;
        GenerateLockType();
    }

    public Command(string lockVersionString)
    {
        var lockVersion = JsonSerializer.Deserialize<LockVersion>(lockVersionString);
        Header = new byte[2];
        Header[0] = 0x7F;
        Header[1] = 0x5A;
        ProtocolType = (byte) lockVersion.getProtocolType();
        _subVersion = (byte) lockVersion.getProtocolVersion();
        _scene = (byte) lockVersion.getScene();
        Organization = DigitUtil.shortToByteArray(lockVersion.getGroupId());
        SubOrganization = DigitUtil.shortToByteArray(lockVersion.getOrgId());
        Encrypt = APP_COMMAND;
        GenerateLockType();
    }

    public Command(LockType lockType)
    {
        var lockVersion = LockVersion.GetLockVersion(lockType);
        Header = new byte[2];
        Header[0] = 0x7F;
        Header[1] = 0x5A;
        ProtocolType = (byte) lockVersion.getProtocolType();
        _subVersion = (byte) lockVersion.getProtocolVersion();
        _scene = (byte) lockVersion.getScene();
        Organization = DigitUtil.shortToByteArray(lockVersion.getGroupId());
        SubOrganization = DigitUtil.shortToByteArray(lockVersion.getOrgId());
        Encrypt = APP_COMMAND;
        if (lockType == LockType.LOCK_TYPE_V2)
        {
            Encrypt = DigitUtil.generateRandomByte();
            Data = new byte[0];
        }

        GenerateLockType();
    }

    public Command(byte[] responseBytes)
    {
        if (responseBytes.Length < 7)
        {
            throw new BaseException("Invalid command length: " + responseBytes.Length);
        }

        Header = new byte[2];
        Header[0] = responseBytes[0];
        Header[1] = responseBytes[1];
        if (Header[0] != 0x7F || Header[1] != 0x5A)
        {
            //throw new BaseException("Invalid header: " + DigitUtil.byteArrayToHexString(Header));
        }

        ProtocolType = responseBytes[2];

        if (ProtocolType >= 5) // new protocol
        {
            Organization = new byte[2];
            SubOrganization = new byte[2];
            _subVersion = responseBytes[3];
            _scene = responseBytes[4];
            Organization[0] = responseBytes[5];
            Organization[1] = responseBytes[6];
            SubOrganization[0] = responseBytes[7];
            SubOrganization[1] = responseBytes[8];
            CommandType = responseBytes[9];
            Encrypt = responseBytes[10];
            Length = responseBytes[11];
            Data = new byte[Length];
            Array.Copy(responseBytes, 12, Data, 0, Length);
        }
        else
        {
            CommandType = responseBytes[3];
            Encrypt = responseBytes[4];
            Length = responseBytes[5];
            Data = new byte[Length];
            Array.Copy(responseBytes, 6, Data, 0, Length);
        }

        this.Checksum = responseBytes[responseBytes.Length - 1];
        var commandWithoutChecksum = new byte[responseBytes.Length - 1];
        Array.Copy(responseBytes, 0, commandWithoutChecksum, 0, commandWithoutChecksum.Length);
        var checksum = CodecUtils.CrcCompute(commandWithoutChecksum);
        _mIsChecksumValid = checksum == this.Checksum;
        Console.WriteLine("checksum=" + checksum + " this.checksum=" + this.Checksum);
        Console.WriteLine("mIsChecksumValid : " + _mIsChecksumValid);
        GenerateLockType();
    }

    public void setCommand(byte command)
    {
        this.CommandType = command;
    }

    public byte getCommand()
    {
        return CommandType;
    }

    public byte getScene()
    {
        return _scene;
    }

    public void setScene(byte scene)
    {
        this._scene = scene;
    }

    public void SetData(byte[] data)
    {
        this.Data = CodecUtils.EncodeWithEncrypt(data, Encrypt);
        Length = (byte) this.Data.Length;
    }

    public byte[] GetData()
    {
        byte[] values;
        values = CodecUtils.DecodeWithEncrypt(Data, Encrypt);
        return values;
    }

    public byte[] GetData(byte[] aesKeyArray)
    {
        if (Data == null)
        {
            throw new ArgumentNullException(nameof(Data));
        }

        byte[] values;
        values = AESUtil.AesDecrypt(Data, aesKeyArray);
        return values;
    }

    public void setData(byte[] data, byte[] aesKeyArray)
    {
        this.Data = AESUtil.AesEncrypt(data, aesKeyArray);
        if (this.Data.Length > 255)
        {
            throw new BaseException("Data length is too long: " + this.Data.Length);
        }

        Length = (byte) this.Data.Length;
    }

    public bool isChecksumValid()
    {
        return _mIsChecksumValid;
    }

    public LockType getLockType()
    {
        return _lockType;
    }

    public void SetLockType(LockType lockType)
    {
        this._lockType = lockType;
    }

    public string GetLockVersionString()
    {
        return JsonSerializer.Serialize(GetLockVersion());
    }

    public LockVersion GetLockVersion()
    {
        var org = DigitUtil.byteArrayToShort(Organization);
        var sub_org = DigitUtil.byteArrayToShort(SubOrganization);
        var lockVersion =
            new LockVersion((sbyte) ProtocolType, (sbyte) _subVersion, (sbyte) _scene, org, sub_org);
        return lockVersion;
    }

    private void GenerateLockType()
    {
        if (ProtocolType == 0x05 && _subVersion == 0x03 && _scene == 0x07)
            SetLockType(LockType.LOCK_TYPE_V3_CAR);
        else if (ProtocolType == 0x0a && _subVersion == 0x01)
            SetLockType(LockType.LOCK_TYPE_CAR);
        else if (ProtocolType == 0x05 && _subVersion == 0x03)
            SetLockType(LockType.LOCK_TYPE_V3);
        else if (ProtocolType == 0x05 && _subVersion == 0x04)
            SetLockType(LockType.LOCK_TYPE_V2S_PLUS);
        else if (ProtocolType == 0x05 && _subVersion == 0x01)
            SetLockType(LockType.LOCK_TYPE_V2S);
        else if (ProtocolType == 0x0b && _subVersion == 0x01)
            SetLockType(LockType.LOCK_TYPE_MOBI);
        else if (ProtocolType == 0x03)
            SetLockType(LockType.LOCK_TYPE_V2);
    }

    /**
   -----------------------------------------------------------------------------------------------------
   |         HEADER         |      DATA      |             DATA PACKET 2              |  DP 3  |  TAIL |
   |7f5a0503020001000155aa20|7c247a4d52ac7ee8 90a9158c42380dca524ffafd927212d375681e97 3dcf670a|59 0d0a|
   -----------------------------------------------------------------------------------------------------

   Bytes (in DEC):
   00 = "Header[0]"
   01 = "Header[1]"
   02 = "Protocol Type"
   03 = "Sub Version" - lockVersion.getProtocolVersion()
   04 = "Scene"
   05 = "Organization[0]" - lockVersion.getGroupId()
   06 = "Organization[1]" - |
   07 = "Sub organization[0]" - lockVersion.getOrgId()
   08 = "Sub organization[1]" - |
   09 = "Command" (ID?)
   10 = "Encrypt" (Byte?)
   11 = "Length" (Of whole data)
   12 = Actual Data

   PACKET TAIL
   "59 0d 0a" = "59" is the CRC byte of whole header+data
   (in this case begins from 7f and ends at 670a) in CRC8/MAXIM format
   and "0d 0a" occurs at the end of every packet,
   which is a carriage return + line feed (CR+LF) in ASCII.
     */
    public byte[] BuildCommand()
    {
        if (ProtocolType >= 0x05)
        {
            var commandWithoutChecksum = new byte[2 + 1 + 1 + 1 + 2 + 2 + 1 + 1 + 1 + Length];
            commandWithoutChecksum[0] = Header[0];
            commandWithoutChecksum[1] = Header[1];
            commandWithoutChecksum[2] = ProtocolType;
            commandWithoutChecksum[3] = _subVersion;
            commandWithoutChecksum[4] = _scene;
            commandWithoutChecksum[5] = Organization[0];
            commandWithoutChecksum[6] = Organization[1];
            commandWithoutChecksum[7] = SubOrganization[0];
            commandWithoutChecksum[8] = SubOrganization[1];
            commandWithoutChecksum[9] = CommandType;
            commandWithoutChecksum[10] = Encrypt;
            commandWithoutChecksum[11] = Length;

            if (Data != null && Data.Length > 0)
                Array.Copy(Data, 0, commandWithoutChecksum, 12, Data.Length);

            var commandWithChecksum = new byte[commandWithoutChecksum.Length + 1];
            var checksum = CodecUtils.CrcCompute(commandWithoutChecksum);
            Array.Copy(commandWithoutChecksum, 0, commandWithChecksum, 0, commandWithoutChecksum.Length);
            commandWithChecksum[commandWithChecksum.Length - 1] = checksum;

            return commandWithChecksum;
        }
        else // V4 and earlier versions
        {
            var commandWithoutChecksum = new byte[2 + 1 + 1 + 1 + 1 + Length];
            commandWithoutChecksum[0] = Header[0];
            commandWithoutChecksum[1] = Header[1];
            commandWithoutChecksum[2] = ProtocolType;
            commandWithoutChecksum[3] = CommandType;
            commandWithoutChecksum[4] = Encrypt;
            commandWithoutChecksum[5] = Length;

            if (Data != null && Data.Length > 0)
                Array.Copy(Data, 0, commandWithoutChecksum, 6, Data.Length);

            var commandWithChecksum = new byte[commandWithoutChecksum.Length + 1];
            var checksum = CodecUtils.CrcCompute(commandWithoutChecksum);
            Array.Copy(commandWithoutChecksum, 0, commandWithChecksum, 0, commandWithoutChecksum.Length);
            commandWithChecksum[commandWithChecksum.Length - 1] = checksum;

            return commandWithChecksum;
        }
    }

    public void SetCommandType(CommandType commandType)
    {
        CommandType = (byte) commandType;
    }


    public static Command From(TTDevice device, AbstractCommand command)
    {
        var cmd = new Command(device.GetLockType());
        cmd.SetCommandType(command.GetCommandType());
        cmd.setData(command.Build(), device.GetAesKeyArray());
        return cmd;
    }

    public void Check(byte[] aesKeyArray)
    {
        if (Data == null) return;
        var data = GetData(aesKeyArray);
    }

    public CommandType GetCommandType()
    {
        return (CommandType) CommandType;
    }

    public void Validate()
    {
        if (!isChecksumValid())
        {
            throw new InvalidChecksumException("Invalid checksum");
        }
    }
}