using SmartHire.Infrastructure.BlobStorage;

namespace SmartHire.Infrastructure.BackgroundServices;

/// <summary>
/// Periodically scans the existing Blob Storage container, extracts text from new/modified
/// resumes, chunks content, generates embeddings, and upserts vectors into Azure AI Search.
/// </summary>
public class ResumeIndexingBackgroundService : BackgroundService
{
    private readonly ILogger<ResumeIndexingBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromMinutes(15);

    public ResumeIndexingBackgroundService(
        ILogger<ResumeIndexingBackgroundService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunIndexingCycleAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resume indexing cycle failed");
            }

            await Task.Delay(PollingInterval, stoppingToken);
        }
    }

    private async Task RunIndexingCycleAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var blobStorageService = scope.ServiceProvider.GetRequiredService<IBlobStorageService>();

        _logger.LogInformation("Starting resume indexing cycle");

        // TODO (implementation phase):
        // 1. List resumes via IBlobStorageService.ListResumesAsync
        // 2. Diff against ResumeDocuments table (new / modified via ContentHash)
        // 3. Extract text (PDF/DOCX), chunk content
        // 4. Generate embeddings via IOpenAiService
        // 5. Upsert vectors via IVectorSearchService
        // 6. Record IndexingJob + update ResumeDocument.Status

        var resumes = await blobStorageService.ListResumesAsync(cancellationToken);
        _logger.LogInformation("Discovered {Count} resumes in blob storage", resumes.Count);
    }
}
