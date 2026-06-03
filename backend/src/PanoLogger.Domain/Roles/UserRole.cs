namespace PanoLogger.Domain.Roles;

public sealed class UserRole
{
    public required Guid UserId { get; init; }
    public required Guid RoleId { get; init; }
    public DateTimeOffset AssignedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
