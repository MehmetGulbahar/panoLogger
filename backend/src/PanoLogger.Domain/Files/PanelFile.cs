using PanoLogger.Domain.Common;

namespace PanoLogger.Domain.Files;

public sealed class PanelFile : Entity
{
    public required Guid PanelId { get; init; }
    public required PanelFileCategory Category { get; init; }
    public required string FileName { get; init; }
    public required string StoragePath { get; init; }
    public required string ContentType { get; init; }
    public required long SizeBytes { get; init; }
}

public enum PanelFileCategory
{
    ElectricalProject = 1,
    MaintenanceReport = 2,
    PanelDocument = 3
}
