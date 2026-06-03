namespace PanoLogger.Application.Common.Interfaces;

public interface IQrCodeService
{
    string CreateSvg(string content, int pixelsPerModule = 8);
}
