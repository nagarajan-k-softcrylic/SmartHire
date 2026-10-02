namespace SmartHire.Entities;

/// <summary>
/// Metadata record for a vector embedding stored in Azure AI Search.
/// The actual vector lives in the search index; this row tracks sync state.
/// </summary>
public class ResumeEmbedding
{
    public int Id { get; set; }

    public int ResumeChunkId { get; set; }
    public ResumeChunk? ResumeChunk { get; set; }

    public string SearchIndexDocumentId { get; set; } = string.Empty;
    public string EmbeddingModel { get; set; } = string.Empty; // e.g. text-embedding-3-large
    public int VectorDimensions { get; set; }
    public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
}
