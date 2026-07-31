using Microsoft.AspNetCore.Mvc;
using thefirey33_backend.Types.Database.Response;

namespace thefirey33_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class InformationController
{
    [HttpGet]
    public InformationResponse Get()
    {
        return new InformationResponse
        {
            MachineName = Environment.MachineName,
            OsName = Environment.OSVersion.VersionString,
            Uptime = Environment.CpuUsage.UserTime.ToString()
        };
    }
}