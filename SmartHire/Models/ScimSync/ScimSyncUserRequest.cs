namespace SmartHire.Models.ScimSync;

/// <summary>
/// Payload pushed by ScimProvisioning.Api to upsert/deactivate a SmartHire user record
/// whenever AuthBridge provisions, updates, or removes that user via SCIM.
/// </summary>
public class ScimSyncUserRequest
{
    /// <summary>The SCIM provisioning record's own id (ScimProvisioning.Api's ProvisionedUser.Id).</summary>
    public string ExternalId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? GivenName { get; set; }

    public string? FamilyName { get; set; }

    public bool Active { get; set; } = true;

    public List<string> Groups { get; set; } = new();
}
