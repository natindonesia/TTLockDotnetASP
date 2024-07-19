namespace Shared.Enums;

public enum LockType : byte
{
    UNKNOWN = 0,
    LOCK_TYPE_V1 = 1,
    /** 3.0 */
    LOCK_TYPE_V2 = 2,
    /** 5.1 */
    LOCK_TYPE_V2S = 3,
    /** 5.4 */
    LOCK_TYPE_V2S_PLUS = 4,
    /** Third generation lock 5.3 */
    LOCK_TYPE_V3 = 5,
    /** Parking lock a.1 */
    LOCK_TYPE_CAR = 6,
    /** Third generation parking lock 5.3.7 */
    LOCK_TYPE_V3_CAR = 8,
    /** Electric car lock b.1 */
    LOCK_TYPE_MOBI = 7,

    /** Remote control equipment 5.3.10 */
    LOCK_TYPE_REMOTE_CONTROL_DEVICE = 9,
    //    /** safe lock */
    LOCK_TYPE_SAFE_LOCK = 8,
    //    /** bicycle lock */
    LOCK_TYPE_BICYCLE = 9
}
