using Shared.Enums;

namespace Shared;

public class LockVersion
{
    public static readonly LockVersion? lockVersion_V2S_PLUS = new LockVersion(5, 4, 1, 1, 1);
    public static readonly LockVersion? lockVersion_V3 = new LockVersion(5, 3, 1, 1, 1);
    public static readonly LockVersion? lockVersion_V2S = new LockVersion(5, 1, 1, 1, 1);
    /**
     * The second-generation parking lock scene is also changed to 7
     */
    public static readonly LockVersion? lockVersion_Va = new LockVersion(0x0a, 1, 0x07, 1, 1);
    /**
     * The electric car lock scene will be changed to 1 and there is no electric car lock
     */
    public static readonly LockVersion? lockVersion_Vb = new LockVersion(0x0b, 1, 0x01, 1, 1);
    public static readonly LockVersion? lockVersion_V2 = new LockVersion(3, 0, 0, 0, 0);
    public static readonly LockVersion? lockVersion_V3_car = new LockVersion(5, 3, 7, 1, 1);

    private int protocolType;
    private int protocolVersion;
    private int scene;
    private int groupId;
    private int orgId;

    public LockVersion(int protocolType, int protocolVersion, int scene, int groupId, int orgId)
    {
        this.protocolType = protocolType;
        this.protocolVersion = protocolVersion;
        this.scene = scene;
        this.groupId = groupId;
        this.orgId = orgId;
    }

    public int GetProtocolType() => protocolType;
    public void SetProtocolType(int protocolType) => this.protocolType = protocolType;

    public int GetProtocolVersion() => protocolVersion;
    public void SetProtocolVersion(int protocolVersion) => this.protocolVersion = protocolVersion;

    public int GetScene() => scene;
    public void SetScene(int scene) => this.scene = scene;

    public int GetGroupId() => groupId;
    public void SetGroupId(int groupId) => this.groupId = groupId;

    public int GetOrgId() => orgId;
    public void SetOrgId(int orgId) => this.orgId = orgId;

    public static LockVersion? GetLockVersion(LockType lockType)
    {
        return lockType switch
        {
            LockType.LOCK_TYPE_V3_CAR => lockVersion_V3_car,
            LockType.LOCK_TYPE_V3 => lockVersion_V3,
            LockType.LOCK_TYPE_V2S_PLUS => lockVersion_V2S_PLUS,
            LockType.LOCK_TYPE_V2S => lockVersion_V2S,
            LockType.LOCK_TYPE_CAR => lockVersion_Va,
            LockType.LOCK_TYPE_MOBI => lockVersion_Vb,
            LockType.LOCK_TYPE_V2 => lockVersion_V2,
            _ => null,
        };
    }

    public static LockType GetLockType(TTDevice ttDevice)
    {
        if (ttDevice.LockType == LockType.UNKNOWN)
        {
            if (ttDevice.ProtocolType == 5 && ttDevice.ProtocolVersion == 3 && ttDevice.Scene == 7)
            {
                ttDevice.LockType = LockType.LOCK_TYPE_V3_CAR;
            }
            else if (ttDevice.ProtocolType == 10 && ttDevice.ProtocolVersion == 1)
            {
                ttDevice.LockType = LockType.LOCK_TYPE_CAR;
            }
            else if (ttDevice.ProtocolType == 11 && ttDevice.ProtocolVersion == 1)
            {
                ttDevice.LockType = LockType.LOCK_TYPE_MOBI;
            }
            else if (ttDevice.ProtocolType == 5 && ttDevice.ProtocolVersion == 4)
            {
                ttDevice.LockType = LockType.LOCK_TYPE_V2S_PLUS;
            }
            else if (ttDevice.ProtocolType == 5 && ttDevice.ProtocolVersion == 3)
            {
                ttDevice.LockType = LockType.LOCK_TYPE_V3;
            }
            else if ((ttDevice.ProtocolType == 5 && ttDevice.ProtocolVersion == 1) ||
                      (ttDevice.Name != null && ttDevice.Name.ToUpper().StartsWith("LOCK_")))
            {
                ttDevice.LockType = LockType.LOCK_TYPE_V2S;
            }
        }
        return ttDevice.LockType;
    }

    public override string ToString()
    {
        return $"{protocolType},{protocolVersion},{scene},{groupId},{orgId}";
    }
}
