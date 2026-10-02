using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHire.Data;
using SmartHire.Entities;
using SmartHire.Infrastructure.Auth;
using SmartHire.Infrastructure.ScimSync;
using SmartHire.Models.ScimSync;

namespace SmartHire.Controllers;

/// <summary>
/// Internal, service-to-service endpoint consumed only by ScimProvisioning.Api to keep
/// SmartHire's own User/UserApplication records in sync with AuthBridge-driven SCIM
/// provisioning events. Not part of the public SmartHire API surface.
/// </summary>
[ApiController]
[Route("internal/scim-sync/users")]
[Authorize(AuthenticationSchemes = ScimSyncApiKeyAuthenticationOptions.SchemeName)]
public class ScimSyncController : ControllerBase
{
    private readonly SmartHireDbContext _db;

    public ScimSyncController(SmartHireDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Upserts a SmartHire user record from a SCIM provisioning event. Matched first by
    /// ScimExternalId (repeat syncs for the same SCIM user), falling back to UserName/Email
    /// so this doesn't collide with existing local accounts.
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] ScimSyncUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ExternalId) || string.IsNullOrWhiteSpace(request.UserName))
        {
            return BadRequest(new { message = "externalId and userName are required." });
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.ScimExternalId == request.ExternalId);

        if (user is null)
        {
            var conflicting = await _db.Users.FirstOrDefaultAsync(u =>
                u.ScimExternalId == null && (u.Username == request.UserName || u.Email == request.Email));
            if (conflicting is not null)
            {
                return Conflict(new
                {
                    message = "An existing local (non-SCIM) account already uses this username or email."
                });
            }

            user = new User
            {
                ScimExternalId = request.ExternalId,
                Username = request.UserName,
            };
            _db.Users.Add(user);
        }

        user.Email = request.Email;
        user.DisplayName = BuildDisplayName(request);
        user.IsActive = request.Active;

        await _db.SaveChangesAsync();
        await SyncGroupsAsync(user, request.Groups);
        await _db.SaveChangesAsync();

        return Ok(new { id = user.Id, scimExternalId = user.ScimExternalId });
    }

    private async Task SyncGroupsAsync(User user, IReadOnlyCollection<string> groups)
    {
        var app = await _db.Applications.FirstOrDefaultAsync(a => a.Code == AuthServiceExtensions.RequiredAppAccessValue);
        if (app is null)
        {
            return;
        }

        var existing = await _db.UserApplications
            .Where(ua => ua.UserId == user.Id && ua.ApplicationId == app.Id)
            .ToListAsync();

        if (groups.Count == 0)
        {
            if (existing.Count == 0)
            {
                _db.UserApplications.Add(new UserApplication { UserId = user.Id, ApplicationId = app.Id });
            }
            return;
        }

        var toRemove = existing.Where(ua => !groups.Contains(ua.Role)).ToList();
        _db.UserApplications.RemoveRange(toRemove);

        foreach (var role in groups)
        {
            if (!existing.Any(ua => ua.Role == role))
            {
                _db.UserApplications.Add(new UserApplication { UserId = user.Id, ApplicationId = app.Id, Role = role });
            }
        }
    }

    private static string BuildDisplayName(ScimSyncUserRequest request)
    {
        var combined = string.Join(' ', new[] { request.GivenName, request.FamilyName }
            .Where(p => !string.IsNullOrWhiteSpace(p)));
        return string.IsNullOrWhiteSpace(combined) ? request.UserName : combined;
    }
}
