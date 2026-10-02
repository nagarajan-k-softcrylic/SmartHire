namespace SmartHire.Entities;

/// <summary>
/// AI-generated ranking/explanation result for a candidate against a screening request.
/// </summary>
public class ScreeningResult
{
    public int Id { get; set; }

    public int ScreeningRequestId { get; set; }
    public ScreeningRequest? ScreeningRequest { get; set; }

    public int CandidateProfileId { get; set; }
    public CandidateProfile? CandidateProfile { get; set; }

    public double MatchPercentage { get; set; }
    public double SkillMatchScore { get; set; }
    public double ExperienceMatchScore { get; set; }
    public string? Strengths { get; set; }
    public string? MissingSkills { get; set; }
    public string? AiRecommendation { get; set; }
    public int Rank { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
