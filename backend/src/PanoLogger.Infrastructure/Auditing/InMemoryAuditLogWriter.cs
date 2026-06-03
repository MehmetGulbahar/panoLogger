using Microsoft.Extensions.Logging;
using PanoLogger.Application.Common.Interfaces;

namespace PanoLogger.Infrastructure.Auditing;

public sealed class InMemoryAuditLogWriter(ILogger<InMemoryAuditLogWriter> logger) : IAuditLogWriter
{
    public Task WriteAsync(string action, string entityName, string entityId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Audit action {Action} for {EntityName}/{EntityId}", action, entityName, entityId);
        return Task.CompletedTask;
    }
}
