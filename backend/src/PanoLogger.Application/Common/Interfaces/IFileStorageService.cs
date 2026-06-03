namespace PanoLogger.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<StoredFileResult> UploadAsync(FileUploadRequest request, CancellationToken cancellationToken = default);
    Task<StoredFileDownload> DownloadAsync(string storagePath, CancellationToken cancellationToken = default);
    Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default);
    Task<SignedUrlResult> CreateSignedUrlAsync(string storagePath, TimeSpan? expiresIn = null, CancellationToken cancellationToken = default);
}

public sealed record FileUploadRequest(
    string StoragePath,
    string FileName,
    string ContentType,
    long SizeBytes,
    Stream Content);

public sealed record StoredFileResult(
    string StoragePath,
    string PublicUrl,
    string ContentType,
    long SizeBytes);

public sealed record StoredFileDownload(
    string StoragePath,
    string FileName,
    string ContentType,
    Stream Content);

public sealed record SignedUrlResult(
    string StoragePath,
    string SignedUrl,
    DateTimeOffset ExpiresAtUtc);
