namespace SmartHire.Infrastructure.BlobStorage;

public record BlobResumeInfo(
    string BlobName,
    string BlobUrl,
    long SizeInBytes,
    DateTimeOffset LastModifiedUtc,
    string ContentHash);

/// <summary>
/// Read-only access to the existing resume container. No upload capability by design.
/// </summary>
public interface IBlobStorageService
{
    Task<IReadOnlyList<BlobResumeInfo>> ListResumesAsync(CancellationToken cancellationToken = default);

    Task<Stream> DownloadResumeAsync(string blobName, CancellationToken cancellationToken = default);
}
