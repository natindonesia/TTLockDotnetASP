using Shared.Enums;

namespace Shared;

public class LockVersion
{
    public static readonly LockVersion lockVersion_V2S_PLUS = new LockVersion(5, 4, 1, 1, 1);
    public static readonly LockVersion lockVersion_V3 = new LockVersion(5, 3, 1, 1, 1);
    public static readonly LockVersion lockVersion_V2S = new LockVersion(5, 1, 1, 1, 1);

    /**
     * The second-generation parking lock scene is also changed to 7
     */
    public static readonly LockVersion lockVersion_Va = new LockVersion(0x0a, 1, 0x07, 1, 1);

    /**
     * The electric car lock scene will be changed to 1 and there is no electric car lock
     */
    public static readonly LockVersion lockVersion_Vb = new LockVersion(0x0b, 1, 0x01, 1, 1);

    public static readonly LockVersion lockVersion_V2 = new LockVersion(3, 0, 0, 0, 0);
    public static readonly LockVersion lockVersion_V3_car = new LockVersion(5, 3, 7, 1, 1);
    private short groupId;
    private short orgId;

    private sbyte protocolType;
    private sbyte protocolVersion;
    private sbyte scene;

    public LockVersion(sbyte protocolType, sbyte protocolVersion, sbyte scene, short groupId, short orgId)
    {
        this.protocolType = protocolType;
        this.protocolVersion = protocolVersion;
        this.scene = scene;
        this.groupId = groupId;
        this.orgId = orgId;
    }

    public sbyte getProtocolType() => protocolType;
    public void setProtocolType(sbyte protocolType) => this.protocolType = protocolType;

    public sbyte getProtocolVersion() => protocolVersion;
    public void setProtocolVersion(sbyte protocolVersion) => this.protocolVersion = protocolVersion;

    public sbyte getScene() => scene;
    public void setScene(sbyte scene) => this.scene = scene;

    public short getGroupId() => groupId;
    public void setGroupId(short groupId) => this.groupId = groupId;

    public short getOrgId() => orgId;
    public void setOrgId(short orgId) => this.orgId = orgId;

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