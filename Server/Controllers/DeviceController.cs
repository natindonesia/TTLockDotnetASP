using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Shared;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class DeviceController : ControllerBase
{
    private readonly ILockManagerService _lockManagerService;

    public DeviceController(ILockManagerService lockManagerService)
    {
        _lockManagerService = lockManagerService;
    }

    [HttpGet(Name = "GetDevices")]
    public Task<IAsyncEnumerable<TTDevice>> GetDevicesAsync()
    {
        return Task.FromResult(_lockManagerService.GetDevicesAsync());
    }
}