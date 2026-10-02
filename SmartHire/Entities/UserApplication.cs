namespace SmartHire.Entities;

/// <summary>
/// Join entity granting a User access to an Application (claim-based authorization cache).
/// </summary>
public class UserApplication
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int ApplicationId { get; set; }
    public Application? Application { get; set; }

    public string Role { get; set; } = "User";
    public DateTime GrantedAtUtc { get; set; } = DateTime.UtcNow;
}
