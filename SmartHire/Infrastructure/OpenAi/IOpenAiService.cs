namespace SmartHire.Infrastructure.OpenAi;

public record CandidateAnalysisResult(
    double MatchPercentage,
    double SkillMatchScore,
    double ExperienceMatchScore,
    IReadOnlyList<string> Strengths,
    IReadOnlyList<string> MissingSkills,
    string Recommendation,
    string ResumeSummary);

/// <summary>
/// Azure OpenAI integration for embeddings (RAG) and GPT-based candidate analysis.
/// No fine-tuning or custom model training is used.
/// </summary>
public interface IOpenAiService
{
    Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);

    Task<CandidateAnalysisResult> AnalyzeCandidateAsync(
        string jobDescription,
        string requiredSkills,
        string resumeContent,
        CancellationToken cancellationToken = default);
}
