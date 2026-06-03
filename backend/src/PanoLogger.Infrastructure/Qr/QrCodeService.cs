using PanoLogger.Application.Common.Interfaces;
using QRCoder;

namespace PanoLogger.Infrastructure.Qr;

public sealed class QrCodeService : IQrCodeService
{
    public string CreateSvg(string content, int pixelsPerModule = 8)
    {
        using var generator = new QRCodeGenerator();
        using var qrCodeData = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        var svgQrCode = new SvgQRCode(qrCodeData);

        return svgQrCode.GetGraphic(pixelsPerModule);
    }
}
