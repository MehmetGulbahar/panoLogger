namespace PanoLogger.Domain.Roles;

public sealed class RolePermission
{
    public required Guid RoleId { get; init; }
    public required string Permission { get; init; }
    public DateTimeOffset GrantedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
