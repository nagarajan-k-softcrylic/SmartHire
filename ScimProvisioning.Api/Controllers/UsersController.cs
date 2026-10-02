using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScimProvisioning.Api.Data;
using ScimProvisioning.Api.Entities;
using ScimProvisioning.Api.Infrastructure;
using ScimProvisioning.Api.Models;

namespace ScimProvisioning.Api.Controllers;

/// <summary>
/// Inbound SCIM 2.0 provisioning endpoint (RFC 7643/7644) consumed by AuthBridge
/// to create and manage user accounts in this standalone provisioning store.
/// </summary>
[ApiController]
[Route("scim/v2/Users")]
[Produces("application/scim+json")]
[Authorize(AuthenticationSchemes = ScimApiKeyAuthenticationOptions.SchemeName)]
public class UsersController : ControllerBase
{
    private readonly ScimDbContext _db;
    private readonly SmartHireSyncClient _smartHireSync;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ScimDbContext db, SmartHireSyncClient smartHireSync, ILogger<UsersController> logger)
    {
        _db = db;
        _smartHireSync = smartHireSync;
        _logger = logger;
    }

    /// <summary>
    /// Fires the SmartHire sync without awaiting it on the SCIM response path, so a SmartHire
    /// outage never causes AuthBridge's SCIM call to fail. Exceptions are already caught and
    /// logged inside SmartHireSyncClient; this just guards against any unexpected throw.
    /// </summary>
    private void FireSmartHireSync(ProvisionedUser user)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await _smartHireSync.SyncUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error firing SmartHire sync for provisioned user {UserId}.", user.Id);
            }
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ScimUser request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            return ScimErrorResult(400, "userName is required.");
        }

        var primaryEmail = request.Emails.FirstOrDefault(e => e.Primary)?.Value
            ?? request.Emails.FirstOrDefault()?.Value
            ?? request.UserName;

        var exists = await _db.Users.AnyAsync(u => u.UserName == request.UserName || u.Email == primaryEmail);
        if (exists)
        {
            return ScimErrorResult(409, "A user with this userName or email already exists.", "uniqueness");
        }

        var user = new ProvisionedUser
        {
            UserName = request.UserName,
            Email = primaryEmail,
            GivenName = request.Name?.GivenName,
            FamilyName = request.Name?.FamilyName,
            Active = request.Active,
        };

        SyncGroups(user, request.Groups, replaceAll: true);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        FireSmartHireSync(user);

        return CreatedAtAction(nameof(Get), new { id = user.Id }, ToScimUser(user));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var user = await FindUserAsync(id);
        if (user is null)
        {
            return ScimErrorResult(404, $"User {id} not found.");
        }

        return Ok(ToScimUser(user));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Replace(string id, [FromBody] ScimUser request)
    {
        var user = await FindUserAsync(id);
        if (user is null)
        {
            return ScimErrorResult(404, $"User {id} not found.");
        }

        var primaryEmail = request.Emails.FirstOrDefault(e => e.Primary)?.Value
            ?? request.Emails.FirstOrDefault()?.Value
            ?? user.Email;

        user.UserName = request.UserName;
        user.Email = primaryEmail;
        user.GivenName = request.Name?.GivenName;
        user.FamilyName = request.Name?.FamilyName;
        user.Active = request.Active;
        user.UpdatedAtUtc = DateTime.UtcNow;

        SyncGroups(user, request.Groups, replaceAll: true);

        await _db.SaveChangesAsync();
        FireSmartHireSync(user);
        return Ok(ToScimUser(user));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(string id, [FromBody] ScimPatchRequest request)
    {
        var user = await FindUserAsync(id);
        if (user is null)
        {
            return ScimErrorResult(404, $"User {id} not found.");
        }

        foreach (var op in request.Operations)
        {
            ApplyPatchOperation(user, op);
        }

        user.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        FireSmartHireSync(user);
        return Ok(ToScimUser(user));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await FindUserAsync(id);
        if (user is null)
        {
            return ScimErrorResult(404, $"User {id} not found.");
        }

        // Soft-delete / deactivate rather than hard-delete.
        user.Active = false;
        user.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        FireSmartHireSync(user);

        return NoContent();
    }

    private async Task<ProvisionedUser?> FindUserAsync(string scimId)
    {
        if (!int.TryParse(scimId, out var id))
        {
            return null;
        }

        return await _db.Users.Include(u => u.Groups).FirstOrDefaultAsync(u => u.Id == id);
    }

    private static void ApplyPatchOperation(ProvisionedUser user, ScimPatchOperation op)
    {
        var opName = op.Op.ToLowerInvariant();
        var path = op.Path?.ToLowerInvariant();

        if (path == "active" && opName == "replace")
        {
            user.Active = op.Value.GetBoolean();
            return;
        }

        if (path == "groups")
        {
            var groups = ParseGroupsValue(op.Value);
            if (opName == "add")
            {
                foreach (var g in groups)
                {
                    if (!user.Groups.Any(ug => ug.Display == g.Display))
                    {
                        user.Groups.Add(new ProvisionedUserGroup { Display = g.Display ?? string.Empty });
                    }
                }
            }
            else if (opName == "remove")
            {
                var displays = groups.Select(g => g.Display).Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
                var toRemove = user.Groups.Where(ug => displays.Contains(ug.Display)).ToList();
                foreach (var g in toRemove)
                {
                    user.Groups.Remove(g);
                }
            }
        }
    }

    private static List<ScimGroup> ParseGroupsValue(System.Text.Json.JsonElement value)
    {
        var groups = new List<ScimGroup>();
        if (value.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            foreach (var item in value.EnumerateArray())
            {
                var display = item.TryGetProperty("display", out var d) ? d.GetString() : null;
                groups.Add(new ScimGroup { Display = display });
            }
        }
        else if (value.ValueKind == System.Text.Json.JsonValueKind.Object &&
                 value.TryGetProperty("display", out var singleDisplay))
        {
            groups.Add(new ScimGroup { Display = singleDisplay.GetString() });
        }

        return groups;
    }

    private static void SyncGroups(ProvisionedUser user, IEnumerable<ScimGroup> groups, bool replaceAll = false)
    {
        if (replaceAll)
        {
            user.Groups.Clear();
        }

        foreach (var group in groups)
        {
            var display = string.IsNullOrWhiteSpace(group.Display) ? "User" : group.Display!;
            if (!user.Groups.Any(g => g.Display == display))
            {
                user.Groups.Add(new ProvisionedUserGroup { Display = display });
            }
        }
    }

    private static ScimUser ToScimUser(ProvisionedUser user) => new()
    {
        Id = user.Id.ToString(),
        UserName = user.UserName,
        Active = user.Active,
        Name = new ScimName { GivenName = user.GivenName, FamilyName = user.FamilyName },
        Emails = new List<ScimEmail> { new() { Value = user.Email, Primary = true } },
        Groups = user.Groups.Select(g => new ScimGroup { Display = g.Display }).ToList()
    };

    private ObjectResult ScimErrorResult(int statusCode, string detail, string? scimType = null)
    {
        return StatusCode(statusCode, ScimError.Create(statusCode, detail, scimType));
    }
}
