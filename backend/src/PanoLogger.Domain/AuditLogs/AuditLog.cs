using PanoLogger.Domain.Common;

namespace PanoLogger.Domain.AuditLogs;

public sealed class AuditLog : Entity
{
    public Guid? UserId { get; init; }
    public required string Action { get; init; }
    public required string EntityName { get; init; }
    public Guid? EntityId { get; init; }
    public required DateTimeOffset OccurredAtUtc { get; init; }
    public required string Metadata { get; init; }
}
