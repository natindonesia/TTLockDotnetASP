namespace Server.Models;

/**
 * Represents ESP device
 */
public interface IEspDevice
{
    public Guid Uuid { get; set; }


    public void OnDeviceNotResponding();
}