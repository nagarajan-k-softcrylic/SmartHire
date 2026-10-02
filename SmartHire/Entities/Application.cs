namespace SmartHire.Entities;
/// <summary>
/// Represents a downstream application registered with AuthBridge (e.g. RESUME_AI).
/// </summary>
public class Application
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // e.g. "RESUME_AI"
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<UserApplication> UserApplications { get; set; } = new List<UserApplication>();
}
