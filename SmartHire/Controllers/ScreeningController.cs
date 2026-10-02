using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Infrastructure.Auth;

namespace SmartHire.Controllers;

public class ScreeningSearchRequest
{
    public string JobTitle { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
    public string RequiredSkills { get; set; } = string.Empty;
    public int MinimumExperienceYears { get; set; }
    public int? PreferredExperienceYears { get; set; }
    public string? Location { get; set; }
}

[ApiController]
[Route("api/screening")]
[Authorize(Policy = AuthServiceExtensions.RequireAppAccessPolicy)]
public class ScreeningController : ControllerBase
{
    // TODO: inject RAG screening orchestration service (embedding -> vector search -> GPT analysis -> ranking).

    [HttpPost("search")]
    public IActionResult Search([FromBody] ScreeningSearchRequest request) => StatusCode(StatusCodes.Status501NotImplemented);

    [HttpPost("rank")]
    public IActionResult Rank([FromBody] ScreeningSearchRequest request) => StatusCode(StatusCodes.Status501NotImplemented);
}
