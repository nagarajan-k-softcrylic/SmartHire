using Azure;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.Options;
using SmartHire.Infrastructure.Options;

namespace SmartHire.Infrastructure.Search;

/// <summary>
/// Azure AI Search backed vector search implementation.
/// NOTE: Index schema / vector profile creation to be finalized during feature implementation.
/// </summary>
public class AzureAiSearchService : IVectorSearchService
{
    private readonly AzureAiSearchOptions _options;
    private readonly SearchIndexClient _indexClient;
    private readonly SearchClient _searchClient;

    public AzureAiSearchService(IOptions<AzureAiSearchOptions> options)
    {
        _options = options.Value;

        var endpoint = new Uri(_options.Endpoint);

        if (_options.UseManagedIdentity)
        {
            var credential = new DefaultAzureCredential();
            _indexClient = new SearchIndexClient(endpoint, credential);
            _searchClient = new SearchClient(endpoint, _options.IndexName, credential);
        }
        else
        {
            var credential = new AzureKeyCredential(_options.ApiKey ?? string.Empty);
            _indexClient = new SearchIndexClient(endpoint, credential);
            _searchClient = new SearchClient(endpoint, _options.IndexName, credential);
        }
    }

    public Task EnsureIndexExistsAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Define vector index schema (content vector field, HNSW profile) during implementation.
        throw new NotImplementedException("Index schema creation will be implemented with the indexing feature.");
    }

    public Task UpsertDocumentsAsync(IEnumerable<ResumeSearchDocument> documents, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Document upsert will be implemented with the indexing feature.");
    }

    public Task<IReadOnlyList<VectorSearchHit>> SearchAsync(float[] queryVector, int topK = 10, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Vector search will be implemented with the screening feature.");
    }
}
