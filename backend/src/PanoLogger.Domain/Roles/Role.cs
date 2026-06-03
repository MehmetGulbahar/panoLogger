using PanoLogger.Domain.Common;

namespace PanoLogger.Domain.Roles;

public sealed class Role : Entity
{
    public required string Name { get; init; }
    public required string Description { get; init; }
}
