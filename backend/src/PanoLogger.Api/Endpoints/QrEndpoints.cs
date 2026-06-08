using System.Security.Claims;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PanoLogger.Api.Authorization;
using PanoLogger.Application.Common.Exceptions;
using PanoLogger.Application.Common.Interfaces;
using PanoLogger.Infrastructure.Persistence;
using PanoLogger.Infrastructure.Qr;

namespace PanoLogger.Api.Endpoints;

public static class QrEndpoints
{
    public static IEndpointRouteBuilder MapQrEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/qr")
            .WithTags("QR Management")
            .RequireAuthorization(AuthorizationPolicies.ManageQrCodes);

        group.MapPost("/panel-codes", async (
            CreatePanelCodeRequest request,
            PanoLoggerDbContext dbContext,
            IPanelCodeService panelCodeService,
            IQrCodeService qrCodeService,
            IOptions<QrCodeOptions> options,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var code = await CreateUniquePanelCodeAsync(
                dbContext,
                panelCodeService,
                request.Prefix ?? options.Value.PanelCodePrefix,
                cancellationToken);

            var publicUrl = BuildPublicPanelUrl(options.Value, code, httpContext);
            return Results.Ok(new GeneratedQrCodeResponse(code, publicUrl, qrCodeService.CreateSvg(publicUrl)));
        })
        .WithName("CreateUniquePanelCode");

        group.MapGet("/panels/{panelId:guid}", async (
            Guid panelId,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IQrCodeService qrCodeService,
            IOptions<QrCodeOptions> options,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var panel = await dbContext.Panels
                .AsNoTracking()
                .Where(item => item.Id == panelId)
                .Join(dbContext.Facilities.AsNoTracking(),
                    panel => panel.FacilityId,
                    facility => facility.Id,
                    (panel, facility) => new { panel.Id, panel.Code, panel.Name, facility.CompanyId })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException($"Panel '{panelId}' was not found.");
            var tenant = await TenantAccessResolver.ResolveAsync(principal, dbContext, cancellationToken);
            tenant.EnsureCompany(panel.CompanyId);

            var publicUrl = BuildPublicPanelUrl(options.Value, panel.Code, httpContext);
            return Results.Ok(new QrCodeResponse(panel.Id, panel.Code, panel.Name, publicUrl, qrCodeService.CreateSvg(publicUrl)));
        })
        .WithName("GetPanelQrCode");

        group.MapGet("/panels/{panelId:guid}/download", async (
            Guid panelId,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IQrCodeService qrCodeService,
            IOptions<QrCodeOptions> options,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var panel = await dbContext.Panels
                .AsNoTracking()
                .Where(item => item.Id == panelId)
                .Join(dbContext.Facilities.AsNoTracking(),
                    panel => panel.FacilityId,
                    facility => facility.Id,
                    (panel, facility) => new { panel.Code, facility.CompanyId })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException($"Panel '{panelId}' was not found.");
            var tenant = await TenantAccessResolver.ResolveAsync(principal, dbContext, cancellationToken);
            tenant.EnsureCompany(panel.CompanyId);

            var publicUrl = BuildPublicPanelUrl(options.Value, panel.Code, httpContext);
            var bytes = Encoding.UTF8.GetBytes(qrCodeService.CreateSvg(publicUrl));

            return Results.File(bytes, "image/svg+xml", $"{panel.Code}-qr.svg");
        })
        .WithName("DownloadPanelQrCode");

        group.MapGet("/panels/{panelId:guid}/print", async (
            Guid panelId,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IQrCodeService qrCodeService,
            IOptions<QrCodeOptions> options,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var panel = await GetPrintablePanelAsync(dbContext, panelId, cancellationToken)
                ?? throw new NotFoundException($"Panel '{panelId}' was not found.");
            var tenant = await TenantAccessResolver.ResolveAsync(principal, dbContext, cancellationToken);
            tenant.EnsureCompany(panel.CompanyId);

            var publicUrl = BuildPublicPanelUrl(options.Value, panel.Code, httpContext);
            var html = BuildPrintHtml(panel.Name, panel.Code, panel.FacilityName, publicUrl, qrCodeService.CreateSvg(publicUrl));

            return Results.Content(html, "text/html; charset=utf-8");
        })
        .WithName("PrintPanelQrCode");

        group.MapPost("/panels/{panelId:guid}/regenerate", async (
            Guid panelId,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IPanelCodeService panelCodeService,
            IQrCodeService qrCodeService,
            IOptions<QrCodeOptions> options,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var companyId = await (
                from panel in dbContext.Panels.AsNoTracking()
                join facility in dbContext.Facilities.AsNoTracking() on panel.FacilityId equals facility.Id
                where panel.Id == panelId
                select facility.CompanyId
            ).FirstOrDefaultAsync(cancellationToken);
            if (companyId == Guid.Empty)
            {
                throw new NotFoundException($"Panel '{panelId}' was not found.");
            }
            var tenant = await TenantAccessResolver.ResolveAsync(principal, dbContext, cancellationToken);
            tenant.EnsureCompany(companyId);

            var code = await CreateUniquePanelCodeAsync(
                dbContext,
                panelCodeService,
                options.Value.PanelCodePrefix,
                cancellationToken);

            await dbContext.Panels
                .Where(item => item.Id == panelId)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(item => item.Code, code),
                    cancellationToken);

            var publicUrl = BuildPublicPanelUrl(options.Value, code, httpContext);
            return Results.Ok(new QrCodeResponse(panelId, code, "", publicUrl, qrCodeService.CreateSvg(publicUrl)));
        })
        .WithName("RegeneratePanelQrCode");

        return app;
    }

    private static async Task<string> CreateUniquePanelCodeAsync(
        PanoLoggerDbContext dbContext,
        IPanelCodeService panelCodeService,
        string? prefix,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            var code = panelCodeService.CreatePanelCode(prefix);
            var isUsed = await dbContext.Panels.AnyAsync(panel => panel.Code == code, cancellationToken);

            if (!isUsed)
            {
                return code;
            }
        }

        throw new InvalidOperationException("Could not generate a unique panel code.");
    }

    private static string BuildPublicPanelUrl(QrCodeOptions options, string panelCode, HttpContext httpContext)
    {
        var publicAppBaseUrl = ResolvePublicAppBaseUrl(options, httpContext);
        return $"{publicAppBaseUrl.TrimEnd('/')}/p/{Uri.EscapeDataString(panelCode)}";
    }

    private static string ResolvePublicAppBaseUrl(QrCodeOptions options, HttpContext httpContext)
    {
        var origin = httpContext.Request.Headers.Origin.FirstOrDefault();
        if (IsHttpOrigin(origin))
        {
            return origin!;
        }

        var referer = httpContext.Request.Headers.Referer.FirstOrDefault();
        if (Uri.TryCreate(referer, UriKind.Absolute, out var refererUri)
            && (refererUri.Scheme == Uri.UriSchemeHttp || refererUri.Scheme == Uri.UriSchemeHttps))
        {
            return $"{refererUri.Scheme}://{refererUri.Authority}";
        }

        return options.PublicAppBaseUrl;
    }

    private static bool IsHttpOrigin(string? origin)
    {
        return Uri.TryCreate(origin, UriKind.Absolute, out var originUri)
            && (originUri.Scheme == Uri.UriSchemeHttp || originUri.Scheme == Uri.UriSchemeHttps);
    }

    private static Task<PrintablePanel?> GetPrintablePanelAsync(
        PanoLoggerDbContext dbContext,
        Guid panelId,
        CancellationToken cancellationToken)
    {
        return (
            from panel in dbContext.Panels.AsNoTracking()
            join facility in dbContext.Facilities.AsNoTracking() on panel.FacilityId equals facility.Id
            where panel.Id == panelId
            select new PrintablePanel(panel.Name, panel.Code, facility.Name, facility.CompanyId)
        ).FirstOrDefaultAsync(cancellationToken);
    }

    private static string BuildPrintHtml(string panelName, string panelCode, string facilityName, string publicUrl, string svg)
    {
        return $$"""
<!doctype html>
<html lang="tr">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>{{WebUtility.HtmlEncode(panelCode)}} QR</title>
  <style>
    body { font-family: Arial, sans-serif; margin: 32px; color: #111827; }
    main { width: 420px; border: 1px solid #d1d5db; padding: 24px; }
    h1 { margin: 0 0 6px; font-size: 24px; }
    p { margin: 4px 0; color: #4b5563; }
    .qr { margin: 20px 0; }
    .code { font-size: 20px; font-weight: 700; color: #111827; }
    @media print { body { margin: 0; } main { border: 0; } }
  </style>
</head>
<body>
  <main>
    <h1>{{WebUtility.HtmlEncode(panelName)}}</h1>
    <p>{{WebUtility.HtmlEncode(facilityName)}}</p>
    <div class="qr">{{svg}}</div>
    <p class="code">{{WebUtility.HtmlEncode(panelCode)}}</p>
    <p>{{WebUtility.HtmlEncode(publicUrl)}}</p>
  </main>
  <script>window.print();</script>
</body>
</html>
""";
    }
}

public sealed record CreatePanelCodeRequest(string? Prefix);

public sealed record GeneratedQrCodeResponse(string Code, string PublicUrl, string Svg);

public sealed record QrCodeResponse(Guid PanelId, string PanelCode, string PanelName, string PublicUrl, string Svg);

public sealed record PrintablePanel(string Name, string Code, string FacilityName, Guid CompanyId);
