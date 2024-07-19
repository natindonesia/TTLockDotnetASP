namespace Shared.Exceptions;

public class ServiceNotFoundException : BaseException
{
    public string Uuid { get; set; }
    public ServiceNotFoundException(string uuid) : base($"Service with UUID {uuid} not found")
    {
        Uuid = uuid;
    }
}
