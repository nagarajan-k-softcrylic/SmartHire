namespace SmartHire.Entities;

/// <summary>
/// Represents an authenticated user provisioned via AuthBridge SSO.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string? SubjectId { get; set; } // "sub" claim from OIDC (null for local accounts)
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Unique login name for local (non-SSO) username/password accounts.</summary>
    public string? Username { get; set; }

    /// <summary>Hashed password for local accounts, produced by ASP.NET Core's PasswordHasher.</summary>
    public string? PasswordHash { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAtUtc { get; set; }

    public ICollection<UserApplication> UserApplications { get; set; } = new List<UserApplication>();
}
