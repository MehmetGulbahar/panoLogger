using PanoLogger.Domain.Common;

namespace PanoLogger.Domain.Companies;

public sealed class Company : Entity
{
    public required string Name { get; init; }
    public required string ProjectName { get; init; }
    public required string TaxNumber { get; init; }
    public required string Address { get; init; }
    public required string ContactEmail { get; init; }
}
