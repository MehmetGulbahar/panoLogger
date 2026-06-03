namespace PanoLogger.Application.Common.Interfaces;

public interface IPanelCodeService
{
    string CreatePanelCode(string? prefix = null);
}
