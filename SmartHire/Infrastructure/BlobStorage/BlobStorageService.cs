using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using SmartHire.Infrastructure.Options;

namespace SmartHire.Infrastructure.BlobStorage;

/// <summary>
/// Reads candidate resumes from the existing Azure Blob Storage container.
/// Supports either Managed Identity or connection-string authentication.
/// </summary>
public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IOptions<AzureBlobStorageOptions> options)
    {
        var settings = options.Value;

        _containerClient = settings.UseManagedIdentity
            ? new BlobContainerClient(
                new Uri($"https://{settings.StorageAccountName}.blob.core.windows.net/{settings.ContainerName}"),
                new DefaultAzureCredential())
            : new BlobContainerClient(settings.ConnectionString, settings.ContainerName);
    }

    public async Task<IReadOnlyList<BlobResumeInfo>> ListResumesAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<BlobResumeInfo>();

        await foreach (var blobItem in _containerClient.GetBlobsAsync(cancellationToken: cancellationToken))
        {
            var extension = Path.GetExtension(blobItem.Name).ToLowerInvariant();
            if (extension is not (".pdf" or ".docx"))
            {
                continue;
            }

            var blobClient = _containerClient.GetBlobClient(blobItem.Name);
            results.Add(new BlobResumeInfo(
                BlobName: blobItem.Name,
                BlobUrl: blobClient.Uri.ToString(),
                SizeInBytes: blobItem.Properties.ContentLength ?? 0,
                LastModifiedUtc: blobItem.Properties.LastModified ?? DateTimeOffset.UtcNow,
                ContentHash: blobItem.Properties.ContentHash is { Length: > 0 }
                    ? Convert.ToBase64String(blobItem.Properties.ContentHash)
                    : string.Empty));
        }

        return results;
    }

    public async Task<Stream> DownloadResumeAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        var download = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
        return download.Value.Content;
    }
}
