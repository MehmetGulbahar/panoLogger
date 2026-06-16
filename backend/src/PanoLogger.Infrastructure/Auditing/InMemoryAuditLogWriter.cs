using Microsoft.Extensions.Logging;
using PanoLogger.Application.Common.Interfaces;

namespace PanoLogger.Infrastructure.Auditing;

public sealed class InMemoryAuditLogWriter(ILogger<InMemoryAuditLogWriter> logger) : IAuditLogWriter
{
    public Task WriteAsync(
        string action,
        string entityName,
        string entityId,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Audit action {Action} for {EntityName}/{EntityId} by {UserId}",
            action,
            entityName,
            entityId,
            userId);
        return Task.CompletedTask;
    }
}
