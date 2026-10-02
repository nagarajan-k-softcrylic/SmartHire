using SmartHire.Entities;

namespace SmartHire.Repositories;

/// <summary>
/// Unit of Work coordinating repositories and committing changes in a single transaction.
/// </summary>
public interface IUnitOfWork
{
    IRepository<ResumeDocument> ResumeDocuments { get; }
    IRepository<ResumeChunk> ResumeChunks { get; }
    IRepository<ResumeEmbedding> ResumeEmbeddings { get; }
    IRepository<CandidateProfile> CandidateProfiles { get; }
    IRepository<ScreeningRequest> ScreeningRequests { get; }
    IRepository<ScreeningResult> ScreeningResults { get; }
    IRepository<AuditLog> AuditLogs { get; }
    IRepository<IndexingJob> IndexingJobs { get; }
    IRepository<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
