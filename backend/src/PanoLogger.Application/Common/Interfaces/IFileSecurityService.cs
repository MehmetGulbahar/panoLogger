namespace PanoLogger.Application.Common.Interfaces;

public interface IFileSecurityService
{
    string NormalizeStoragePath(string storagePath);
    void ValidateUpload(string storagePath, string fileName, string contentType, long sizeBytes);
}
