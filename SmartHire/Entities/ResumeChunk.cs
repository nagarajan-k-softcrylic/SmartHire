namespace SmartHire.Entities;

/// <summary>
/// A chunk of extracted resume text used for embedding generation (RAG).
/// </summary>
public class ResumeChunk
{
    public int Id { get; set; }

    public int ResumeDocumentId { get; set; }
    public ResumeDocument? ResumeDocument { get; set; }

    public int ChunkIndex { get; set; }
    public string Content { get; set; } = string.Empty;
    public int TokenCount { get; set; }

    public ResumeEmbedding? Embedding { get; set; }
}
