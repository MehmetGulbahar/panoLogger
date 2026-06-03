using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using PanoLogger.Application.Common.Exceptions;
using PanoLogger.Application.Common.Interfaces;

namespace PanoLogger.Infrastructure.Storage;

public sealed class SupabaseStorageService(
    HttpClient httpClient,
    IOptions<SupabaseStorageOptions> options,
    IFileSecurityService fileSecurityService) : IFileStorageService
{
    private readonly SupabaseStorageOptions _options = options.Value;

    public async Task<StoredFileResult> UploadAsync(FileUploadRequest request, CancellationToken cancellationToken = default)
    {
        EnsureConfigured();
        fileSecurityService.ValidateUpload(request.StoragePath, request.FileName, request.ContentType, request.SizeBytes);
        var storagePath = fileSecurityService.NormalizeStoragePath(request.StoragePath);

        using var content = new StreamContent(request.Content);
        content.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);
        using var uploadRequest = new HttpRequestMessage(HttpMethod.Post, BuildObjectPath(storagePath))
        {
            Content = content
        };
        uploadRequest.Headers.Add("x-upsert", "true");

        using var response = await httpClient.SendAsync(uploadRequest, cancellationToken);

        await EnsureSuccessAsync(response, "upload file", cancellationToken);

        return new StoredFileResult(storagePath, BuildPublicUrl(storagePath), request.ContentType, request.SizeBytes);
    }

    public async Task<StoredFileDownload> DownloadAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        EnsureConfigured();
        var normalizedPath = fileSecurityService.NormalizeStoragePath(storagePath);
        using var response = await httpClient.GetAsync(BuildObjectPath(normalizedPath), cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException($"File '{normalizedPath}' was not found.");
        }

        await EnsureSuccessAsync(response, "download file", cancellationToken);

        var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var memoryStream = new MemoryStream();
        await contentStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        var fileName = Path.GetFileName(normalizedPath);

        return new StoredFileDownload(normalizedPath, fileName, contentType, memoryStream);
    }

    public async Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        EnsureConfigured();
        var normalizedPath = fileSecurityService.NormalizeStoragePath(storagePath);
        using var response = await httpClient.DeleteAsync(BuildObjectPath(normalizedPath), cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return;
        }

        await EnsureSuccessAsync(response, "delete file", cancellationToken);
    }

    public async Task<SignedUrlResult> CreateSignedUrlAsync(
        string storagePath,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();
        var normalizedPath = fileSecurityService.NormalizeStoragePath(storagePath);
        var expires = expiresIn ?? TimeSpan.FromSeconds(_options.SignedUrlExpirationSeconds);
        var payload = new { expiresIn = Math.Max(1, (int)expires.TotalSeconds) };

        using var response = await httpClient.PostAsJsonAsync(
            $"/storage/v1/object/sign/{Uri.EscapeDataString(_options.StorageBucket)}/{EscapePath(normalizedPath)}",
            payload,
            cancellationToken);

        await EnsureSuccessAsync(response, "create signed URL", cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<SupabaseSignedUrlResponse>(cancellationToken);
        if (string.IsNullOrWhiteSpace(result?.SignedUrl))
        {
            throw new InvalidOperationException("Supabase did not return a signed URL.");
        }

        var signedUrl = BuildSignedUrl(result.SignedUrl);

        return new SignedUrlResult(normalizedPath, signedUrl, DateTimeOffset.UtcNow.Add(expires));
    }

    private string BuildSignedUrl(string signedUrl)
    {
        if (signedUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return signedUrl;
        }

        var normalizedPath = signedUrl.StartsWith("/", StringComparison.Ordinal)
            ? signedUrl
            : $"/{signedUrl}";

        if (!normalizedPath.StartsWith("/storage/v1/", StringComparison.OrdinalIgnoreCase))
        {
            normalizedPath = $"/storage/v1{normalizedPath}";
        }

        return $"{_options.Url.TrimEnd('/')}{normalizedPath}";
    }

    private string BuildObjectPath(string storagePath)
    {
        return $"/storage/v1/object/{Uri.EscapeDataString(_options.StorageBucket)}/{EscapePath(storagePath)}";
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_options.Url) || string.IsNullOrWhiteSpace(_options.ServiceRoleKey))
        {
            throw new InvalidOperationException("Supabase storage is not configured. Set Supabase:Url and Supabase:ServiceRoleKey.");
        }
    }

    private string BuildPublicUrl(string storagePath)
    {
        return $"{_options.Url.TrimEnd('/')}/storage/v1/object/public/{Uri.EscapeDataString(_options.StorageBucket)}/{EscapePath(storagePath)}";
    }

    private static string EscapePath(string storagePath)
    {
        return string.Join("/", storagePath.Split('/').Select(Uri.EscapeDataString));
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, string action, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new InvalidOperationException($"Supabase failed to {action}. Status: {(int)response.StatusCode}. Body: {responseBody}");
    }

    private sealed record SupabaseSignedUrlResponse(string? SignedUrl);
}
