using Microsoft.EntityFrameworkCore;
using SmartHire.Data;
using SmartHire.Entities;

namespace SmartHire.Services;

public class DashboardService : IDashboardService
{
    private readonly SmartHireDbContext _context;

    public DashboardService(SmartHireDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummary> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var totalResumes = await _context.ResumeDocuments.CountAsync(cancellationToken);
        var indexedResumes = await _context.ResumeDocuments
            .CountAsync(r => r.Status == IndexingStatus.Indexed, cancellationToken);
        var totalSearches = await _context.ScreeningRequests.CountAsync(cancellationToken);
        var activeJobs = await _context.IndexingJobs
            .CountAsync(j => j.Status == IndexingJobStatus.Running || j.Status == IndexingJobStatus.Queued, cancellationToken);

        return new DashboardSummary(
            TotalResumes: totalResumes,
            IndexedResumes: indexedResumes,
            TotalSearches: totalSearches,
            TopCandidatesCount: 0,
            ActiveProcessingJobs: activeJobs);
    }
}
