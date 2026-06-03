using PanoLogger.Application.Common.Interfaces;

namespace PanoLogger.Infrastructure.Qr;

public sealed class PanelCodeService : IPanelCodeService
{
    public string CreatePanelCode(string? prefix = null)
    {
        var normalizedPrefix = string.IsNullOrWhiteSpace(prefix) ? "PNL" : NormalizePrefix(prefix);
        var randomSegment = Random.Shared.Next(1000, 9999);
        var timeSegment = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()[^5..];

        return $"{normalizedPrefix}-{timeSegment}-{randomSegment}";
    }

    private static string NormalizePrefix(string value)
    {
        var chars = value
            .Trim()
            .ToUpperInvariant()
            .Where(char.IsLetterOrDigit)
            .Take(8)
            .ToArray();

        return chars.Length == 0 ? "PNL" : new string(chars);
    }
}
