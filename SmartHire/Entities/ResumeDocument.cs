namespace SmartHire.Entities;

public enum IndexingStatus
{
    Pending,
    Processing,
    Indexed,
    Failed
}

/// <summary>
/// Represents a resume file that exists in the external Azure Blob Storage container.
/// </summary>
public class ResumeDocument
{
    public int Id { get; set; }
    public string BlobName { get; set; } = string.Empty;
    public string BlobUrl { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty; // .pdf | .docx
    public long SizeInBytes { get; set; }
    public string? ContentHash { get; set; } // used to detect modifications
    public DateTime BlobLastModifiedUtc { get; set; }
    public IndexingStatus Status { get; set; } = IndexingStatus.Pending;
    public DateTime? LastIndexedAtUtc { get; set; }
    public string? FailureReason { get; set; }

    public CandidateProfile? CandidateProfile { get; set; }
    public ICollection<ResumeChunk> Chunks { get; set; } = new List<ResumeChunk>();
}
