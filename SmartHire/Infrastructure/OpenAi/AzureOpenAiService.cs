using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Options;
using SmartHire.Infrastructure.Options;

namespace SmartHire.Infrastructure.OpenAi;

/// <summary>
/// Azure OpenAI backed implementation for embeddings and GPT candidate analysis.
/// </summary>
public class AzureOpenAiService : IOpenAiService
{
    private readonly AzureOpenAiOptions _options;
    private readonly AzureOpenAIClient _client;

    public AzureOpenAiService(IOptions<AzureOpenAiOptions> options)
    {
        _options = options.Value;
        var endpoint = new Uri(_options.Endpoint);

        _client = _options.UseManagedIdentity
            ? new AzureOpenAIClient(endpoint, new DefaultAzureCredential())
            : new AzureOpenAIClient(endpoint, new AzureKeyCredential(_options.ApiKey ?? string.Empty));
    }

    public Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        // TODO: Implement using _client.GetEmbeddingClient(_options.EmbeddingDeploymentName)
        throw new NotImplementedException("Embedding generation will be implemented with the indexing feature.");
    }

    public Task<CandidateAnalysisResult> AnalyzeCandidateAsync(
        string jobDescription,
        string requiredSkills,
        string resumeContent,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement using _client.GetChatClient(_options.ChatDeploymentName) with a scoring prompt.
        throw new NotImplementedException("Candidate analysis will be implemented with the screening feature.");
    }
}
