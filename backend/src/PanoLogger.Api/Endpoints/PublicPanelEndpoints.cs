using Microsoft.EntityFrameworkCore;
using PanoLogger.Application.Common.Exceptions;
using PanoLogger.Application.Common.Interfaces;
using PanoLogger.Domain.Files;
using PanoLogger.Infrastructure.Persistence;

namespace PanoLogger.Api.Endpoints;

public static class PublicPanelEndpoints
{
    public static IEndpointRouteBuilder MapPublicPanelEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/public").WithTags("Public Panel Access");

        group.MapGet("/panels/{panelCode}", async (
            string panelCode,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var panel = await GetPublicPanelAsync(panelCode, dbContext, cancellationToken)
                ?? throw new NotFoundException($"Panel code '{panelCode}' was not found.");

            var storedFiles = await dbContext.PanelFiles
                .AsNoTracking()
                .Where(file => file.PanelId == panel.PanelId)
                .OrderBy(file => file.Category)
                .ThenBy(file => file.FileName)
                .ToListAsync(cancellationToken);

            var files = storedFiles
                .Select(file => new PublicPanelFileResponse(
                    file.Id,
                    file.Category,
                    file.FileName,
                    file.ContentType,
                    file.SizeBytes,
                    $"/api/public/panels/{panel.PanelCode}/files/{file.Id}/view",
                    $"/api/public/panels/{panel.PanelCode}/files/{file.Id}/download"))
                .ToList();

            return Results.Ok(panel with
            {
                Documents = new PublicPanelDocumentsResponse(
                    files.Count(file => file.Category == "ElectricalProject"),
                    files.Count(file => file.Category == "MaintenanceReport"),
                    files.Count(file => file.Category == "PanelDocument"),
                    files.GroupBy(file => file.Category).ToDictionary(group => group.Key, group => group.Count()),
                    files)
            });
        })
        .WithName("GetPublicPanelByCode");

        group.MapGet("/panels/{panelCode}/files/{fileId:guid}/view", async (
            string panelCode,
            Guid fileId,
            PanoLoggerDbContext dbContext,
            IFileStorageService fileStorageService,
            CancellationToken cancellationToken) =>
        {
            return await GetPublicPanelFileAsync(panelCode, fileId, forceDownload: false, dbContext, fileStorageService, cancellationToken);
        })
        .WithName("ViewPublicPanelFile");

        group.MapGet("/panels/{panelCode}/files/{fileId:guid}/download", async (
            string panelCode,
            Guid fileId,
            PanoLoggerDbContext dbContext,
            IFileStorageService fileStorageService,
            CancellationToken cancellationToken) =>
        {
            return await GetPublicPanelFileAsync(panelCode, fileId, forceDownload: true, dbContext, fileStorageService, cancellationToken);
        })
        .WithName("DownloadPublicPanelFile");

        return app;
    }

    private static async Task<PublicPanelResponse?> GetPublicPanelAsync(
        string panelCode,
        PanoLoggerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var panelResult = await (
            from panel in dbContext.Panels.AsNoTracking()
            join facility in dbContext.Facilities.AsNoTracking() on panel.FacilityId equals facility.Id
            join company in dbContext.Companies.AsNoTracking() on facility.CompanyId equals company.Id
            where panel.Code == panelCode
            select new
            {
                PanelId = panel.Id,
                PanelCode = panel.Code,
                PanelName = panel.Name,
                PanelDescription = panel.Description,
                FacilityId = facility.Id,
                FacilityName = facility.Name,
                facility.City,
                CompanyId = company.Id,
                CompanyName = company.Name,
                company.ProjectName
            }
        ).FirstOrDefaultAsync(cancellationToken);

        return panelResult is null
            ? null
            : new PublicPanelResponse(
                panelResult.PanelId,
                panelResult.PanelCode,
                panelResult.PanelName,
                panelResult.PanelDescription,
                panelResult.FacilityId,
                panelResult.FacilityName,
                panelResult.City,
                panelResult.CompanyId,
                panelResult.CompanyName,
                panelResult.ProjectName,
                new PublicPanelDocumentsResponse(0, 0, 0, new Dictionary<string, int>(), Array.Empty<PublicPanelFileResponse>()));
    }

    private static async Task<IResult> GetPublicPanelFileAsync(
        string panelCode,
        Guid fileId,
        bool forceDownload,
        PanoLoggerDbContext dbContext,
        IFileStorageService fileStorageService,
        CancellationToken cancellationToken)
    {
        var file = await (
            from panel in dbContext.Panels.AsNoTracking()
            join panelFile in dbContext.PanelFiles.AsNoTracking() on panel.Id equals panelFile.PanelId
            where panel.Code == panelCode && panelFile.Id == fileId
            select new { panelFile.StoragePath, panelFile.FileName, panelFile.ContentType }
        ).FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException($"File '{fileId}' was not found for panel '{panelCode}'.");

        var storedFile = await fileStorageService.DownloadAsync(file.StoragePath, cancellationToken);

        return Results.File(
            storedFile.Content,
            file.ContentType,
            fileDownloadName: forceDownload ? file.FileName : null,
            enableRangeProcessing: true);
    }
}

public sealed record PublicPanelResponse(
    Guid PanelId,
    string PanelCode,
    string PanelName,
    string PanelDescription,
    Guid FacilityId,
    string FacilityName,
    string City,
    Guid CompanyId,
    string CompanyName,
    string ProjectName,
    PublicPanelDocumentsResponse Documents);

public sealed record PublicPanelDocumentsResponse(
    int ElectricalProjectCount,
    int MaintenanceReportCount,
    int PanelDocumentCount,
    IReadOnlyDictionary<string, int> CategoryCounts,
    IReadOnlyCollection<PublicPanelFileResponse> Files);

public sealed record PublicPanelFileResponse(
    Guid Id,
    string Category,
    string FileName,
    string ContentType,
    long SizeBytes,
    string ViewUrl,
    string DownloadUrl);
