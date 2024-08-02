namespace Shared.Enums;

public static class TTLiftWorkModeExtensions
{
    public static TTLiftWorkMode? GetInstance(int value)
    {
        switch (value)
        {
            case 0x01:
                return TTLiftWorkMode.ActivateAllFloors;
            case 0x02:
                return TTLiftWorkMode.ActivateSpecificFloors;
            default:
                return null;
        }
    }

    public static int GetValue(this TTLiftWorkMode mode)
    {
        return (int) mode;
    }
}