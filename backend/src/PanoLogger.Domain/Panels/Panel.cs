using PanoLogger.Domain.Common;

namespace PanoLogger.Domain.Panels;

public sealed class Panel : Entity
{
    public required Guid FacilityId { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}
