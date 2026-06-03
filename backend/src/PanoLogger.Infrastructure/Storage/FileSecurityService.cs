using Microsoft.Extensions.Options;
using PanoLogger.Application.Common.Exceptions;
using PanoLogger.Application.Common.Interfaces;

namespace PanoLogger.Infrastructure.Storage;

public sealed class FileSecurityService(IOptions<SupabaseStorageOptions> options) : IFileSecurityService
{
    private readonly SupabaseStorageOptions _options = options.Value;

    public string NormalizeStoragePath(string storagePath)
    {
        var normalizedPath = storagePath
            .Replace('\\', '/')
            .Trim()
            .TrimStart('/');

        while (normalizedPath.Contains("//", StringComparison.Ordinal))
        {
            normalizedPath = normalizedPath.Replace("//", "/", StringComparison.Ordinal);
        }

        if (string.IsNullOrWhiteSpace(normalizedPath)
            || normalizedPath.Contains("..", StringComparison.Ordinal)
            || normalizedPath.StartsWith(".", StringComparison.Ordinal)
            || normalizedPath.Contains('\0', StringComparison.Ordinal))
        {
            throw new ValidationException("Invalid storage path.");
        }

        return normalizedPath;
    }

    public void ValidateUpload(string storagePath, string fileName, string contentType, long sizeBytes)
    {
        _ = NormalizeStoragePath(storagePath);

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ValidationException("File name is required.");
        }

        if (sizeBytes <= 0 || sizeBytes > _options.MaxFileBytes)
        {
            throw new ValidationException($"File size must be between 1 byte and {_options.MaxFileBytes} bytes.");
        }

        if (!_options.AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationException($"File content type '{contentType}' is not allowed.");
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension)
            || !_options.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationException($"File extension '{extension}' is not allowed.");
        }
    }
}
