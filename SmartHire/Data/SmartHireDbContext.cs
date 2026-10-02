using Microsoft.EntityFrameworkCore;
using SmartHire.Entities;

namespace SmartHire.Data;

public class SmartHireDbContext : DbContext
{
    public SmartHireDbContext(DbContextOptions<SmartHireDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<UserApplication> UserApplications => Set<UserApplication>();
    public DbSet<ResumeDocument> ResumeDocuments => Set<ResumeDocument>();
    public DbSet<ResumeChunk> ResumeChunks => Set<ResumeChunk>();
    public DbSet<ResumeEmbedding> ResumeEmbeddings => Set<ResumeEmbedding>();
    public DbSet<CandidateProfile> CandidateProfiles => Set<CandidateProfile>();
    public DbSet<ScreeningRequest> ScreeningRequests => Set<ScreeningRequest>();
    public DbSet<ScreeningResult> ScreeningResults => Set<ScreeningResult>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<IndexingJob> IndexingJobs => Set<IndexingJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.SubjectId)
            .IsUnique()
            .HasFilter("[SubjectId] IS NOT NULL");

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique()
            .HasFilter("[Username] IS NOT NULL");

        modelBuilder.Entity<Application>()
            .HasIndex(a => a.Code)
            .IsUnique();

        modelBuilder.Entity<UserApplication>()
            .HasIndex(ua => new { ua.UserId, ua.ApplicationId })
            .IsUnique();

        modelBuilder.Entity<ResumeDocument>()
            .HasIndex(r => r.BlobName)
            .IsUnique();

        modelBuilder.Entity<ResumeDocument>()
            .HasOne(r => r.CandidateProfile)
            .WithOne(c => c.ResumeDocument!)
            .HasForeignKey<CandidateProfile>(c => c.ResumeDocumentId);

        modelBuilder.Entity<ResumeChunk>()
            .HasOne(c => c.ResumeDocument)
            .WithMany(r => r.Chunks)
            .HasForeignKey(c => c.ResumeDocumentId);

        modelBuilder.Entity<ResumeEmbedding>()
            .HasOne(e => e.ResumeChunk)
            .WithOne(c => c.Embedding!)
            .HasForeignKey<ResumeEmbedding>(e => e.ResumeChunkId);

        modelBuilder.Entity<ScreeningResult>()
            .HasOne(sr => sr.ScreeningRequest)
            .WithMany(req => req.Results)
            .HasForeignKey(sr => sr.ScreeningRequestId);

        modelBuilder.Entity<ScreeningResult>()
            .HasOne(sr => sr.CandidateProfile)
            .WithMany(c => c.ScreeningResults)
            .HasForeignKey(sr => sr.CandidateProfileId);

        // Seed the RESUME_AI application so claim-based authorization has a target record.
        modelBuilder.Entity<Application>().HasData(new Application
        {
            Id = 1,
            Code = "RESUME_AI",
            Name = "SmartHire",
            IsActive = true
        });
    }
}
