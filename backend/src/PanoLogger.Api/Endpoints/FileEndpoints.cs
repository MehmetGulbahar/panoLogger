using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PanoLogger.Api.Authorization;
using PanoLogger.Application.Common.Exceptions;
using PanoLogger.Application.Common.Interfaces;
using PanoLogger.Domain.Files;
using PanoLogger.Infrastructure.Persistence;

namespace PanoLogger.Api.Endpoints;

public static class FileEndpoints
{
    public static IEndpointRouteBuilder MapFileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/files")
            .WithTags("Panel Files")
            .RequireAuthorization();

        group.MapGet("/summary", async (
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var tenant = await TenantAccessResolver.ResolveAsync(principal, dbContext, cancellationToken);
            var panelsQuery =
                from panel in dbContext.Panels.AsNoTracking()
                join facility in dbContext.Facilities.AsNoTracking() on panel.FacilityId equals facility.Id
                where tenant.IsSuperAdmin || facility.CompanyId == tenant.CompanyId
                select panel.Id;

            var totalPanels = await panelsQuery.CountAsync(cancellationToken);
            var totalFiles = await dbContext.PanelFiles
                .Where(file => panelsQuery.Contains(file.PanelId))
                .CountAsync(cancellationToken);
            var panelsWithFiles = await dbContext.PanelFiles
                .Where(file => panelsQuery.Contains(file.PanelId))
                .Select(file => file.PanelId)
                .Distinct()
                .CountAsync(cancellationToken);

            return Results.Ok(new FileSummaryResponse(totalFiles, panelsWithFiles, totalPanels));
        })
        .WithName("GetFileSummary");

        group.MapGet("/panels/{panelId:guid}", async (
            Guid panelId,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!await CanAccessPanelAsync(principal, dbContext, panelId, cancellationToken))
            {
                throw new NotFoundException($"Panel '{panelId}' was not found.");
            }

            var files = await dbContext.PanelFiles
                .AsNoTracking()
                .Where(file => file.PanelId == panelId)
                .OrderByDescending(file => file.CreatedAtUtc)
                .Select(file => new PanelFileResponse(
                    file.Id,
                    file.PanelId,
                    file.Category.ToString(),
                    file.FileName,
                    file.ContentType,
                    file.SizeBytes,
                    file.CreatedAtUtc))
                .ToListAsync(cancellationToken);

            return Results.Ok(files);
        })
        .WithName("GetPanelFiles");

        group.MapPost("/panels/{panelId:guid}", async (
            Guid panelId,
            [FromForm] IFormFile file,
            [FromForm] string? category,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IFileStorageService fileStorageService,
            CancellationToken cancellationToken) =>
        {
            if (!await CanAccessPanelAsync(principal, dbContext, panelId, cancellationToken))
            {
                throw new NotFoundException($"Panel '{panelId}' was not found.");
            }

            var parsedCategory = ParseCategory(category);
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var storagePath = $"panels/{panelId:D}/{parsedCategory}/{Guid.NewGuid():N}{extension}";

            await using var content = file.OpenReadStream();
            var storedFile = await fileStorageService.UploadAsync(
                new FileUploadRequest(storagePath, file.FileName, file.ContentType, file.Length, content),
                cancellationToken);

            var panelFile = new PanelFile
            {
                PanelId = panelId,
                Category = parsedCategory,
                FileName = file.FileName,
                StoragePath = storedFile.StoragePath,
                ContentType = storedFile.ContentType,
                SizeBytes = storedFile.SizeBytes,
            };

            try
            {
                dbContext.PanelFiles.Add(panelFile);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await fileStorageService.DeleteAsync(storedFile.StoragePath, cancellationToken);
                throw;
            }

            return Results.Created(
                $"/api/files/{panelFile.Id}",
                new PanelFileResponse(
                    panelFile.Id,
                    panelFile.PanelId,
                    panelFile.Category.ToString(),
                    panelFile.FileName,
                    panelFile.ContentType,
                    panelFile.SizeBytes,
                    panelFile.CreatedAtUtc));
        })
        .DisableAntiforgery()
        .RequireAuthorization(AuthorizationPolicies.ManageFiles)
        .WithName("UploadPanelFile");

        group.MapGet("/{fileId:guid}/download", async (
            Guid fileId,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IFileStorageService fileStorageService,
            CancellationToken cancellationToken) =>
        {
            var storagePath = await dbContext.PanelFiles
                .AsNoTracking()
                .Join(dbContext.Panels.AsNoTracking(),
                    file => file.PanelId,
                    panel => panel.Id,
                    (file, panel) => new { File = file, Panel = panel })
                .Join(dbContext.Facilities.AsNoTracking(),
                    item => item.Panel.FacilityId,
                    facility => facility.Id,
                    (item, facility) => new { item.File, facility.CompanyId })
                .Where(item => item.File.Id == fileId)
                .Select(item => new { item.File.StoragePath, item.CompanyId })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException($"File '{fileId}' was not found.");
            var tenant = await TenantAccessResolver.ResolveAsync(principal, dbContext, cancellationToken);
            tenant.EnsureCompany(storagePath.CompanyId);

            var signedUrl = await fileStorageService.CreateSignedUrlAsync(storagePath.StoragePath, cancellationToken: cancellationToken);
            return Results.Ok(new { signedUrl = signedUrl.SignedUrl, expiresAtUtc = signedUrl.ExpiresAtUtc });
        })
        .WithName("GetPanelFileDownloadUrl");

        return app;
    }

    private static PanelFileCategory ParseCategory(string? category)
    {
        return Enum.TryParse<PanelFileCategory>(category, true, out var parsedCategory)
            ? parsedCategory
            : PanelFileCategory.PanelDocument;
    }

    private static async Task<bool> CanAccessPanelAsync(
        ClaimsPrincipal principal,
        PanoLoggerDbContext dbContext,
        Guid panelId,
        CancellationToken cancellationToken)
    {
        var panelCompanyId = await (
            from panel in dbContext.Panels.AsNoTracking()
            join facility in dbContext.Facilities.AsNoTracking() on panel.FacilityId equals facility.Id
            where panel.Id == panelId
            select facility.CompanyId
        ).FirstOrDefaultAsync(cancellationToken);

        if (panelCompanyId == Guid.Empty)
        {
            return false;
        }

        var tenant = await TenantAccessResolver.ResolveAsync(principal, dbContext, cancellationToken);
        return tenant.CanAccessCompany(panelCompanyId);
    }
}

public sealed record PanelFileResponse(
    Guid Id,
    Guid PanelId,
    string Category,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTimeOffset CreatedAtUtc);

public sealed record FileSummaryResponse(int TotalFiles, int PanelsWithFiles, int TotalPanels);
