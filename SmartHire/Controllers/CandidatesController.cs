using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Infrastructure.Auth;

namespace SmartHire.Controllers;

[ApiController]
[Route("api/candidates")]
[Authorize(Policy = AuthServiceExtensions.RequireAppAccessPolicy)]
public class CandidatesController : ControllerBase
{
    // TODO: inject a candidate query service once the candidate details feature is implemented.

    [HttpGet]
    public IActionResult GetAll() => StatusCode(StatusCodes.Status501NotImplemented);

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id) => StatusCode(StatusCodes.Status501NotImplemented);
}
