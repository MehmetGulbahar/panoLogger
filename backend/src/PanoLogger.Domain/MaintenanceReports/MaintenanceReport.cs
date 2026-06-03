using PanoLogger.Domain.Common;

namespace PanoLogger.Domain.MaintenanceReports;

public sealed class MaintenanceReport : Entity
{
    public required Guid PanelId { get; init; }
    public required string Title { get; init; }
    public required DateTimeOffset ReportDateUtc { get; init; }
    public required string Notes { get; init; }
    public Guid? CreatedByUserId { get; init; }
}
