namespace PanoLogger.Application.Common.Interfaces;

public interface IAuditLogWriter
{
    Task WriteAsync(
        string action,
        string entityName,
        string entityId,
        Guid? userId = null,
        CancellationToken cancellationToken = default);
}
