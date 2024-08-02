namespace Shared.Enums;

public enum TTLiftWorkMode
{
    /// <summary>
    /// Activate all floors: Using a card from this hotel, any floor can be selected manually (1 - Do not control floors, using a card from this hotel, any floor controlled by the elevator can be manually selected, configuration for bytes 2-9 is invalid)
    /// </summary>
    ActivateAllFloors = 0x01,

    /// <summary>
    /// Activate specific floors: Only the floor corresponding to the card is allowed (2 - Control floors, according to the parameters in bytes 2-9: 0 - Control, 1 - Do not control)
    /// </summary>
    ActivateSpecificFloors = 0x02
}