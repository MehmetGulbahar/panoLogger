using PanoLogger.Domain.Common;

namespace PanoLogger.Domain.Files;

public sealed class PanelFile : Entity
{
    public required Guid PanelId { get; init; }
    public required string Category { get; init; }
    public required string FileName { get; init; }
    public required string StoragePath { get; init; }
    public required string ContentType { get; init; }
    public required long SizeBytes { get; init; }
}

public sealed class PanelFileCategory : Entity
{
    public required Guid PanelId { get; init; }
    public required string Key { get; init; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Icon { get; set; }
    public required int SortOrder { get; set; }
    public required bool IsSystem { get; init; }
    public required bool IsActive { get; set; }
}
