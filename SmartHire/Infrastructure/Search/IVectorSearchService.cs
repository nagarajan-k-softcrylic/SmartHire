namespace SmartHire.Infrastructure.Search;

public record ResumeSearchDocument(
    string Id,
    string ResumeDocumentId,
    string CandidateName,
    string Content,
    float[] ContentVector,
    string BlobUrl);

public record VectorSearchHit(string ResumeDocumentId, string Content, string BlobUrl, double Score);

/// <summary>
/// Vector search over the Azure AI Search resume index (RAG retrieval).
/// </summary>
public interface IVectorSearchService
{
    Task EnsureIndexExistsAsync(CancellationToken cancellationToken = default);

    Task UpsertDocumentsAsync(IEnumerable<ResumeSearchDocument> documents, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VectorSearchHit>> SearchAsync(float[] queryVector, int topK = 10, CancellationToken cancellationToken = default);
}
