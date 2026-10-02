using SmartHire.Data;
using SmartHire.Entities;

namespace SmartHire.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SmartHireDbContext _context;

    public UnitOfWork(SmartHireDbContext context)
    {
        _context = context;
        ResumeDocuments = new Repository<ResumeDocument>(context);
        ResumeChunks = new Repository<ResumeChunk>(context);
        ResumeEmbeddings = new Repository<ResumeEmbedding>(context);
        CandidateProfiles = new Repository<CandidateProfile>(context);
        ScreeningRequests = new Repository<ScreeningRequest>(context);
        ScreeningResults = new Repository<ScreeningResult>(context);
        AuditLogs = new Repository<AuditLog>(context);
        IndexingJobs = new Repository<IndexingJob>(context);
        Users = new Repository<User>(context);
    }

    public IRepository<ResumeDocument> ResumeDocuments { get; }
    public IRepository<ResumeChunk> ResumeChunks { get; }
    public IRepository<ResumeEmbedding> ResumeEmbeddings { get; }
    public IRepository<CandidateProfile> CandidateProfiles { get; }
    public IRepository<ScreeningRequest> ScreeningRequests { get; }
    public IRepository<ScreeningResult> ScreeningResults { get; }
    public IRepository<AuditLog> AuditLogs { get; }
    public IRepository<IndexingJob> IndexingJobs { get; }
    public IRepository<User> Users { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
