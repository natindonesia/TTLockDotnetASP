namespace Shared.Enums;

public class LockError
{
    public static readonly LockError SUCCESS = new LockError(0, "success", "success");
    public static readonly LockError LOCK_CRC_CHECK_ERROR = new LockError(0x01, "CRC error", "CRC error");

    public static readonly LockError LOCK_NO_PERMISSION = new LockError(0x02, "Not administrator, has no permission.",
        "Not administrator, has no permission.");

    public static readonly LockError LOCK_ADMIN_CHECK_ERROR =
        new LockError(0x03, "Wrong administrator password.", "Wrong administrator password.");

    public static readonly LockError LOCK_IS_IN_SETTING_MODE =
        new LockError(0x05, "lock is in setting mode", "lock is in setting mode");

    public static readonly LockError LOCK_NOT_EXIST_ADMIN =
        new LockError(0x06, "lock has no administrator", "lock has no administrator");

    public static readonly LockError LOCK_IS_IN_NO_SETTING_MODE =
        new LockError(0x07, "Non-setting mode", "Non-setting mode");

    public static readonly LockError LOCK_DYNAMIC_PWD_ERROR =
        new LockError(0x08, "invalid dynamic code", "invalid dynamic code");

    public static readonly LockError LOCK_NO_POWER = new LockError(0x0a, "run out of battery", "run out of battery");

    public static readonly LockError LOCK_INIT_KEYBOARD_FAILED = new LockError(0x0b,
        "initialize keyboard password falied", "initialize keyboard password falied");

    public static readonly LockError LOCK_KEY_FLAG_INVALID = new LockError(0x0d,
        "invalid ekey, lock flag position is low", "invalid ekey, lock flag position is low");

    public static readonly LockError LOCK_USER_TIME_EXPIRED = new LockError(0x0e, "ekey expired", "ekey expired");

    public static readonly LockError LOCK_PASSWORD_LENGTH_INVALID =
        new LockError(0x0f, "invalid password length", "invalid password length");

    public static readonly LockError LOCK_SUPER_PASSWORD_IS_SAME_WITH_DELETE_PASSWORD = new LockError(0x10,
        "admin super password is same with delete password", "admin super password is same with delete password");

    public static readonly LockError LOCK_USER_TIME_INEFFECTIVE =
        new LockError(0x11, "ekey hasn't become effective", "ekey hasn't become effective");

    public static readonly LockError LOCK_USER_NOT_LOGIN = new LockError(0x12, "user not login", "user not login");

    public static readonly LockError LOCK_OPERATE_FAILED =
        new LockError(0x13, "Failed. Undefined error.", "Failed. Undefined error.");

    public static readonly LockError LOCK_PASSWORD_EXIST =
        new LockError(0x14, "password already exists.", "password already exists.");

    public static readonly LockError LOCK_PASSWORD_NOT_EXIST = new LockError(0x15,
        "password not exist or never be used", "password not exist or never be used");

    public static readonly LockError LOCK_NO_FREE_MEMORY = new LockError(0x16, "out of memory", "out of memory");
    public static readonly LockError NO_DEFINED_ERROR = new LockError(0x17, "undefined error", "undefined error");

    public static readonly LockError IC_CARD_NOT_EXIST =
        new LockError(0x18, "Card number not exist.", "Card number not exist.");

    public static readonly LockError FINGER_PRINT_NOT_EXIST =
        new LockError(0x1a, "Finger print not exist.", "Finger print not exist.");

    public static readonly LockError INVALID_COMMAND = new LockError(0x1b, "Invalid command", "Invalid command");
    public static readonly LockError LOCK_FROZEN = new LockError(0x1c, "lock frozen", "lock frozen");

    public static readonly LockError INVALID_VENDOR =
        new LockError(0x1d, "invalid vendor string", "invalid vendor string");

    public static readonly LockError LOCK_REVERSE = new LockError(0x1e, "double locked", "double locked");
    public static readonly LockError RECORD_NOT_EXIST = new LockError(0x1f, "record not exist", "record not exist");
    public static readonly LockError INVALID_PARAM = new LockError(0x20, "invalid param", "invalid param");

    public static readonly LockError PARKING_LOCK_LOCKED_FAILED = new LockError(0x21,
        "Maybe there are obstacles or cars above the parking lock",
        "Maybe there are obstacles or car above the parking lock");

    public static readonly LockError COMMAND_RECEIVED = new LockError(0x24, "command received", "command received");
    public static readonly LockError BAD_WIFI_NAME = new LockError(0x25, "bad wifi name", "bad wifi name");
    public static readonly LockError BAD_WIFI_PASSWORD = new LockError(0x26, "bad wifi password", "bad wifi password");

    public static readonly LockError Failed = new LockError(-1, "failed", "failed");

    public static readonly LockError AES_PARSE_ERROR = new LockError(0x30, "aes parse error", "aes parse error");

    public static readonly LockError KEY_INVALID =
        new LockError(0x31, "key invalid, may be reset", "key invalid, may be reset");

    public static readonly LockError LOCK_CONNECT_FAIL = new LockError(0x400,
        "lock connect time out or the connection is disconnected",
        "lock connect time out or the connection is disconnected");

    public static readonly LockError LOCK_IS_BUSY =
        new LockError(0x401, "only one command can be proceed at a time", "lock is busy");

    public static readonly LockError DATA_FORMAT_ERROR =
        new LockError(0x403, "parameter format or content is incorrect", "parameter error");

    public static readonly LockError LOCK_IS_NOT_SUPPORT = new LockError(0x404, "lock doesn't support this operation",
        "lock doesn't support this operation");

    public static readonly LockError BLE_SERVER_NOT_INIT =
        new LockError(0x405, "bluetooth is disable", "not init or bluetooth is disable");

    public static readonly LockError SCAN_FAILED_ALREADY_START = new LockError(0x406,
        "fails to start scan as BLE scan with the same settings is already started by the app",
        "BLE scan already started by the app");

    public static readonly LockError SCAN_FAILED_APPLICATION_REGISTRATION_FAILED = new LockError(0x407,
        "fails to start scan as app cannot be registered", "fails to start scan as app cannot be registered");

    public static readonly LockError SCAN_FAILED_INTERNAL_ERROR = new LockError(0x408,
        "fails to start scan due to an internal error", "fails to start scan due an internal error");

    public static readonly LockError SCAN_FAILED_FEATURE_UNSUPPORTED = new LockError(0x409,
        "fails to start power optimized scan as this feature is not supported",
        "fails to start power optimized scan as this feature is not supported");

    public static readonly LockError SCAN_FAILED_OUT_OF_HARDWARE_RESOURCES = new LockError(0x410,
        "fails to start scan as it is out of hardware resources",
        "fails to start scan as it is out of hardware resources");

    public static readonly LockError INIT_WIRELESS_KEYBOARD_FAILED =
        new LockError(0x411, "add keyboard failed", "failed to init wireless keyboard");

    public static readonly LockError WIRELESS_KEYBOARD_NO_RESPONSE =
        new LockError(0x412, "wireless keyboard no response", "time out");

    public static readonly LockError DEVICE_CONNECT_FAILED =
        new LockError(0x413, "device connect time out", "device connect time out");


    private byte command;

    /**
     * error time
     */
    private long date;


    private String description;


    private int errorCode;


    private String errorMsg;


    private String lockmac;


    private String lockname;

    private String sdkLog;

    private LockError(int errorCode, String description, String errorMsg)
    {
        this.errorCode = errorCode;
        this.description = description;
        this.errorMsg = errorMsg;
    }

    public static LockError getInstance(int errorCode)
    {
        return errorCode switch
        {
            0 => SUCCESS,
            0x01 => LOCK_CRC_CHECK_ERROR,
            0x02 => LOCK_NO_PERMISSION,
            0x03 => LOCK_ADMIN_CHECK_ERROR,
            0x05 => LOCK_IS_IN_SETTING_MODE,
            0x06 => LOCK_NOT_EXIST_ADMIN,
            0x07 => LOCK_IS_IN_NO_SETTING_MODE,
            0x08 => LOCK_DYNAMIC_PWD_ERROR,
            0x0a => LOCK_NO_POWER,
            0x0b => LOCK_INIT_KEYBOARD_FAILED,
            0x0d => LOCK_KEY_FLAG_INVALID,
            0x0e => LOCK_USER_TIME_EXPIRED,
            0x0f => LOCK_PASSWORD_LENGTH_INVALID,
            0x10 => LOCK_SUPER_PASSWORD_IS_SAME_WITH_DELETE_PASSWORD,
            0x11 => LOCK_USER_TIME_INEFFECTIVE,
            0x12 => LOCK_USER_NOT_LOGIN,
            0x13 => LOCK_OPERATE_FAILED,
            0x14 => LOCK_PASSWORD_EXIST,
            0x15 => LOCK_PASSWORD_NOT_EXIST,
            0x16 => LOCK_NO_FREE_MEMORY,
            0x17 => NO_DEFINED_ERROR,
            0x18 => IC_CARD_NOT_EXIST,
            0x1a => FINGER_PRINT_NOT_EXIST,
            0x1b => INVALID_COMMAND,
            0x1c => LOCK_FROZEN,
            0x1d => INVALID_VENDOR,
            0x1e => LOCK_REVERSE,
            0x1f => RECORD_NOT_EXIST,
            0x20 => INVALID_PARAM,
            0x21 => PARKING_LOCK_LOCKED_FAILED,
            0x24 => COMMAND_RECEIVED,
            0x25 => BAD_WIFI_NAME,
            0x26 => BAD_WIFI_PASSWORD,
            _ => Failed
        };
    }

    public String getLockname()
    {
        return lockname;
    }

    public Exception getException()
    {
        return new Exception(String.Format("errorCode: {0}, errorMsg: {1}, description: {2}", errorCode, errorMsg,
            description));
    }

    public void setLockname(String lockname)
    {
        this.lockname = lockname;
    }

    public String getLockmac()
    {
        return lockmac;
    }

    public void setLockmac(String lockmac)
    {
        this.lockmac = lockmac;
    }

    public String getCommand()
    {
        if (command >= 'A' && command <= 'Z')
        {
            return ((char) command).ToString();
        }

        return command.ToString("X");
    }

    public void setCommand(byte command)
    {
        this.command = command;
    }

    public long getDate()
    {
        return date;
    }

    public void setDate(long date)
    {
        this.date = date;
    }

    public String getErrorMsg()
    {
        return errorMsg;
    }


    public String getDescription()
    {
        return description;
    }


    public String getErrorCode()
    {
        return errorCode.ToString("X");
    }

    public int getIntErrorCode()
    {
        return errorCode;
    }

    public String getSdkLog()
    {
        return sdkLog;
    }

    public void setSdkLog(String sdkLog)
    {
        this.sdkLog = sdkLog;
    }
}