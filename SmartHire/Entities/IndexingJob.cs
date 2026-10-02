namespace SmartHire.Entities;

public enum IndexingJobStatus
{
    Queued,
    Running,
    Completed,
    CompletedWithErrors,
    Failed
}

/// <summary>
/// Tracks a single run of the background resume synchronization/indexing process.
/// </summary>
public class IndexingJob
{
    public int Id { get; set; }
    public IndexingJobStatus Status { get; set; } = IndexingJobStatus.Queued;
    public int DocumentsDiscovered { get; set; }
    public int DocumentsIndexed { get; set; }
    public int DocumentsFailed { get; set; }
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorSummary { get; set; }
}
