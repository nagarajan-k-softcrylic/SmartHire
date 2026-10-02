using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Infrastructure.Auth;

namespace SmartHire.Controllers;

[ApiController]
[Route("api/resumes")]
[Authorize(Policy = AuthServiceExtensions.RequireAppAccessPolicy)]
public class ResumesController : ControllerBase
{
    // TODO: inject a resumes query service once the resume listing feature is implemented.

    [HttpGet]
    public IActionResult GetAll() => StatusCode(StatusCodes.Status501NotImplemented);

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id) => StatusCode(StatusCodes.Status501NotImplemented);
}
