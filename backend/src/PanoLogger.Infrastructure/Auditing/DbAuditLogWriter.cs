using PanoLogger.Application.Common.Interfaces;
using PanoLogger.Domain.AuditLogs;
using PanoLogger.Infrastructure.Persistence;

namespace PanoLogger.Infrastructure.Auditing;

public sealed class DbAuditLogWriter(PanoLoggerDbContext dbContext) : IAuditLogWriter
{
    public async Task WriteAsync(
        string action,
        string entityName,
        string entityId,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var parsedEntityId = Guid.TryParse(entityId, out var id) ? id : (Guid?)null;

        dbContext.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = parsedEntityId,
            OccurredAtUtc = DateTimeOffset.UtcNow,
            Metadata = "{}",
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
