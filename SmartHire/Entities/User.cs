namespace SmartHire.Entities;

/// <summary>
/// Represents an authenticated user provisioned via AuthBridge SSO.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string SubjectId { get; set; } = string.Empty; // "sub" claim from OIDC
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAtUtc { get; set; }

    public ICollection<UserApplication> UserApplications { get; set; } = new List<UserApplication>();
}
