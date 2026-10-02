namespace ScimProvisioning.Api.Entities;

/// <summary>
/// A user account provisioned into this system via an inbound SCIM 2.0 request
/// from an identity provider (e.g. AuthBridge). This store is independent of any
/// downstream application's own user table.
/// </summary>
public class ProvisionedUser
{
    public int Id { get; set; }

    /// <summary>SCIM "userName" - typically the user's login/email.</summary>
    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? GivenName { get; set; }

    public string? FamilyName { get; set; }

    /// <summary>SCIM "active" - soft-delete / enable-disable flag.</summary>
    public bool Active { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<ProvisionedUserGroup> Groups { get; set; } = new List<ProvisionedUserGroup>();
}

/// <summary>Role/group membership assigned to a provisioned user via SCIM.</summary>
public class ProvisionedUserGroup
{
    public int Id { get; set; }

    public int ProvisionedUserId { get; set; }

    public ProvisionedUser? ProvisionedUser { get; set; }

    /// <summary>SCIM group "display" value (e.g. role name).</summary>
    public string Display { get; set; } = string.Empty;
}
