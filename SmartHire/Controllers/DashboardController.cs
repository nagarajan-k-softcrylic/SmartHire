using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Infrastructure.Auth;
using SmartHire.Services;

namespace SmartHire.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Policy = AuthServiceExtensions.RequireAppAccessPolicy)]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var summary = await _dashboardService.GetSummaryAsync(cancellationToken);
        return Ok(summary);
    }
}
