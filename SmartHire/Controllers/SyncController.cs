using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Infrastructure.Auth;

namespace SmartHire.Controllers;

[ApiController]
[Route("api/sync")]
[Authorize(Policy = AuthServiceExtensions.RequireAppAccessPolicy)]
public class SyncController : ControllerBase
{
    // TODO: inject indexing/job services once the synchronization feature is implemented.

    [HttpPost("start")]
    public IActionResult Start() => StatusCode(StatusCodes.Status501NotImplemented);

    [HttpPost("reindex")]
    public IActionResult Reindex() => StatusCode(StatusCodes.Status501NotImplemented);

    [HttpGet("status")]
    public IActionResult Status() => StatusCode(StatusCodes.Status501NotImplemented);

    [HttpGet("jobs")]
    public IActionResult Jobs() => StatusCode(StatusCodes.Status501NotImplemented);
}
