namespace Shared.Exceptions;

public class CharacteristicNotFoundException : BaseException
{
    public string Uuid { get; set; }
    public CharacteristicNotFoundException(string uuid) : base($"Characteristic with UUID {uuid} not found")
    {
        Uuid = uuid;
    }
}
