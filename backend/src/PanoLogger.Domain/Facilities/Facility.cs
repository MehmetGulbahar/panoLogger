using PanoLogger.Domain.Common;

namespace PanoLogger.Domain.Facilities;

public sealed class Facility : Entity
{
    public required Guid CompanyId { get; init; }
    public required string Name { get; init; }
    public required string City { get; init; }
    public required string District { get; init; }
    public required string Address { get; init; }
}
