using System.Text.Json;
using Shared.Enums;
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
    static readonly int ENCRY_OLD = 1;
    static readonly int ENCRY_AES_CBC = 2;
    public byte checksum; // 校验		1 字节
    public byte command; // 命令字 	1 字节
    public byte[] data; // 数据
    public byte encrypt; // 加密字		1 字节

    public byte[] header; // 帧首 		2 字节
    public byte length; // 长度		1 字节

    /**
     * 锁类型
     */
    private LockType lockType;

    private bool mIsChecksumValid;
    public byte[] organization;
    public byte protocol_type; // 预留	 	1 字节 //reserved 修改为protocol_type
    private byte scene;
    public byte[] sub_organization;

    // V4版本新加
    private byte sub_version;

    public Command(LockVersion lockVersion)
    {
        header = new byte[2];
        header[0] = 0x7F;
        header[1] = 0x5A;
        protocol_type = (byte) lockVersion.getProtocolType();
        sub_version = (byte) lockVersion.getProtocolVersion();
        scene = (byte) lockVersion.getScene();
        organization = DigitUtil.shortToByteArray(lockVersion.getGroupId());
        sub_organization = DigitUtil.shortToByteArray(lockVersion.getOrgId());
        encrypt = APP_COMMAND;
        generateLockType();
    }

    public Command(String lockVersionString)
    {
        LockVersion lockVersion = JsonSerializer.Deserialize<LockVersion>(lockVersionString);
        header = new byte[2];
        header[0] = 0x7F;
        header[1] = 0x5A;
        protocol_type = (byte) lockVersion.getProtocolType();
        sub_version = (byte) lockVersion.getProtocolVersion();
        scene = (byte) lockVersion.getScene();
        organization = DigitUtil.shortToByteArray(lockVersion.getGroupId());
        sub_organization = DigitUtil.shortToByteArray(lockVersion.getOrgId());
        encrypt = APP_COMMAND;
        generateLockType();
    }

    public Command(LockType lockType)
    {
        LockVersion lockVersion = LockVersion.GetLockVersion(lockType);
        header = new byte[2];
        header[0] = 0x7F;
        header[1] = 0x5A;
        protocol_type = (byte) lockVersion.getProtocolType();
        sub_version = (byte) lockVersion.getProtocolVersion();
        scene = (byte) lockVersion.getScene();
        organization = DigitUtil.shortToByteArray(lockVersion.getGroupId());
        sub_organization = DigitUtil.shortToByteArray(lockVersion.getOrgId());
        encrypt = APP_COMMAND;
        if (lockType == LockType.LOCK_TYPE_V2)
        {
            this.encrypt = DigitUtil.generateRandomByte();
            data = new byte[0];
        }

        generateLockType();
    }

    public Command(byte[] command)
    {
        this.header = new byte[2];
        this.header[0] = command[0];
        this.header[1] = command[1];
        this.protocol_type = command[2];
        try
        {
            if (this.protocol_type >= 5) // new protocol
            {
                organization = new byte[2];
                sub_organization = new byte[2];
                this.sub_version = command[3];
                this.scene = command[4];
                this.organization[0] = command[5];
                this.organization[1] = command[6];
                this.sub_organization[0] = command[7];
                this.sub_organization[1] = command[8];
                this.command = command[9];
                this.encrypt = command[10];
                this.length = command[11];
                this.data = new byte[this.length];
                Array.Copy(command, 12, this.data, 0, this.length);
            }
            else
            {
                this.command = command[3];
                this.encrypt = command[4];
                this.length = command[5];
                this.data = new byte[this.length];
                Array.Copy(command, 6, this.data, 0, this.length);
            }

            this.checksum = command[command.Length - 1];
            byte[] commandWithoutChecksum = new byte[command.Length - 1];
            Array.Copy(command, 0, commandWithoutChecksum, 0, commandWithoutChecksum.Length);
            byte checksum = CodecUtils.CrcCompute(commandWithoutChecksum);
            mIsChecksumValid = (checksum == this.checksum);
            Console.WriteLine("checksum=" + checksum + " this.checksum=" + this.checksum);
            Console.WriteLine("mIsChecksumValid : " + mIsChecksumValid);
            generateLockType();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void setCommand(byte command)
    {
        this.command = command;
    }

    public byte getCommand()
    {
        return command;
    }

    public byte getScene()
    {
        return scene;
    }

    public void setScene(byte scene)
    {
        this.scene = scene;
    }

    public void setData(byte[] data)
    {
        this.data = CodecUtils.EncodeWithEncrypt(data, this.encrypt);
        this.length = (byte) this.data.Length;
    }

    public byte[] getData()
    {
        byte[] values;
        values = CodecUtils.DecodeWithEncrypt(data, encrypt);
        return values;
    }

    public byte[] getData(byte[] aesKeyArray)
    {
        byte[] values;
        values = AESUtil.AESEncrypt(data, aesKeyArray);
        return values;
    }

    public void setData(byte[] data, byte[] aesKeyArray)
    {
        Console.WriteLine("data=" + DigitUtil.byteArrayToHexString(data), DBG);
        Console.WriteLine("aesKeyArray=" + DigitUtil.byteArrayToHexString(aesKeyArray), DBG);
        this.data = AESUtil.AESEncrypt(data, aesKeyArray);
        this.length = (byte) this.data.Length;
    }

    public bool isChecksumValid()
    {
        return mIsChecksumValid;
    }

    public LockType getLockType()
    {
        return lockType;
    }

    public void setLockType(LockType lockType)
    {
        this.lockType = lockType;
    }

    public String getLockVersionString()
    {
        return JsonSerializer.Serialize(getLockVersion());
    }

    public LockVersion getLockVersion()
    {
        short org = DigitUtil.byteArrayToShort(organization);
        short sub_org = DigitUtil.byteArrayToShort(sub_organization);
        LockVersion lockVersion =
            new LockVersion((sbyte) protocol_type, (sbyte) sub_version, (sbyte) scene, org, sub_org);
        return lockVersion;
    }

    private void generateLockType()
    {
        if (protocol_type == 0x05 && sub_version == 0x03 && scene == 0x07)
            setLockType(LockType.LOCK_TYPE_V3_CAR);
        else if (protocol_type == 0x0a && sub_version == 0x01)
            setLockType(LockType.LOCK_TYPE_CAR);
        else if (protocol_type == 0x05 && sub_version == 0x03)
            setLockType(LockType.LOCK_TYPE_V3);
        else if (protocol_type == 0x05 && sub_version == 0x04)
            setLockType(LockType.LOCK_TYPE_V2S_PLUS);
        else if (protocol_type == 0x05 && sub_version == 0x01)
            setLockType(LockType.LOCK_TYPE_V2S);
        else if (protocol_type == 0x0b && sub_version == 0x01)
            setLockType(LockType.LOCK_TYPE_MOBI);
        else if (protocol_type == 0x03)
            setLockType(LockType.LOCK_TYPE_V2);
    }

    public byte[] buildCommand()
    {
        if (protocol_type >= 0x05)
        {
            byte[] commandWithoutChecksum = new byte[2 + 1 + 1 + 1 + 2 + 2 + 1 + 1 + 1 + this.length];
            commandWithoutChecksum[0] = this.header[0];
            commandWithoutChecksum[1] = this.header[1];
            commandWithoutChecksum[2] = this.protocol_type;
            commandWithoutChecksum[3] = this.sub_version;
            commandWithoutChecksum[4] = this.scene;
            commandWithoutChecksum[5] = this.organization[0];
            commandWithoutChecksum[6] = this.organization[1];
            commandWithoutChecksum[7] = this.sub_organization[0];
            commandWithoutChecksum[8] = this.sub_organization[1];
            commandWithoutChecksum[9] = this.command;
            commandWithoutChecksum[10] = this.encrypt;
            commandWithoutChecksum[11] = this.length;

            if (this.data != null && this.data.Length > 0)
                Array.Copy(this.data, 0, commandWithoutChecksum, 12, this.data.Length);

            byte[] commandWithChecksum = new byte[commandWithoutChecksum.Length + 1];
            byte checksum = CodecUtils.CrcCompute(commandWithoutChecksum);
            Array.Copy(commandWithoutChecksum, 0, commandWithChecksum, 0, commandWithoutChecksum.Length);
            commandWithChecksum[commandWithChecksum.Length - 1] = checksum;

            Console.WriteLine("BuildCommand : " + (char) command + "-" + string.Format("{0:#x}", command), DBG);
            return commandWithChecksum;
        }
        else // V4 and earlier versions
        {
            byte[] commandWithoutChecksum = new byte[2 + 1 + 1 + 1 + 1 + this.length];
            commandWithoutChecksum[0] = this.header[0];
            commandWithoutChecksum[1] = this.header[1];
            commandWithoutChecksum[2] = this.protocol_type;
            commandWithoutChecksum[3] = this.command;
            commandWithoutChecksum[4] = this.encrypt;
            commandWithoutChecksum[5] = this.length;

            if (this.data != null && this.data.Length > 0)
                Array.Copy(this.data, 0, commandWithoutChecksum, 6, this.data.Length);

            byte[] commandWithChecksum = new byte[commandWithoutChecksum.Length + 1];
            byte checksum = CodecUtils.CrcCompute(commandWithoutChecksum);
            Array.Copy(commandWithoutChecksum, 0, commandWithChecksum, 0, commandWithoutChecksum.Length);
            commandWithChecksum[commandWithChecksum.Length - 1] = checksum;

            Console.WriteLine("BuildCommand : " + (char) command, DBG);
            return commandWithChecksum;
        }
    }
}