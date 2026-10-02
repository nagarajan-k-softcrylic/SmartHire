namespace SmartHire.Entities;

/// <summary>
/// A recruiter-submitted job screening request (RAG query input).
/// </summary>
public class ScreeningRequest
{
    public int Id { get; set; }

    public int RequestedByUserId { get; set; }
    public User? RequestedByUser { get; set; }

    public string JobTitle { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
    public string RequiredSkills { get; set; } = string.Empty;
    public int MinimumExperienceYears { get; set; }
    public int? PreferredExperienceYears { get; set; }
    public string? Location { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<ScreeningResult> Results { get; set; } = new List<ScreeningResult>();
}
