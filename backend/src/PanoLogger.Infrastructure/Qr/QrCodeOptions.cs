namespace PanoLogger.Infrastructure.Qr;

public sealed class QrCodeOptions
{
    public const string SectionName = "Qr";

    public string PublicAppBaseUrl { get; init; } = "http://localhost:5173";
    public string PanelCodePrefix { get; init; } = "PNL";
}
