namespace SmartHire.Services;

public record DashboardSummary(
    int TotalResumes,
    int IndexedResumes,
    int TotalSearches,
    int TopCandidatesCount,
    int ActiveProcessingJobs);

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync(CancellationToken cancellationToken = default);
}
