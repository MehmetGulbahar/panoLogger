using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        group.MapGet("/panels/{panelId:guid}", async (
            Guid panelId,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var files = await dbContext.PanelFiles
                .AsNoTracking()
                .Where(file => file.PanelId == panelId)
                .OrderByDescending(file => file.CreatedAtUtc)
                .Select(file => new PanelFileResponse(
                    file.Id,
                    file.PanelId,
                    file.Category,
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
            PanoLoggerDbContext dbContext,
            IFileStorageService fileStorageService,
            CancellationToken cancellationToken) =>
        {
            if (!await dbContext.Panels.AnyAsync(panel => panel.Id == panelId, cancellationToken))
            {
                throw new NotFoundException($"Panel '{panelId}' was not found.");
            }

            var parsedCategory = await ParseCategoryAsync(dbContext, category, cancellationToken);
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
                    panelFile.Category,
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
            PanoLoggerDbContext dbContext,
            IFileStorageService fileStorageService,
            CancellationToken cancellationToken) =>
        {
            var storagePath = await dbContext.PanelFiles
                .AsNoTracking()
                .Where(file => file.Id == fileId)
                .Select(file => file.StoragePath)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException($"File '{fileId}' was not found.");

            var signedUrl = await fileStorageService.CreateSignedUrlAsync(storagePath, cancellationToken: cancellationToken);
            return Results.Ok(new { signedUrl = signedUrl.SignedUrl, expiresAtUtc = signedUrl.ExpiresAtUtc });
        })
        .WithName("GetPanelFileDownloadUrl");

        group.MapDelete("/{fileId:guid}", async (
            Guid fileId,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IFileStorageService fileStorageService,
            CancellationToken cancellationToken) =>
        {
            var file = await (
                from panelFile in dbContext.PanelFiles
                join panel in dbContext.Panels on panelFile.PanelId equals panel.Id
                join facility in dbContext.Facilities on panel.FacilityId equals facility.Id
                where panelFile.Id == fileId
                select new
                {
                    PanelFile = panelFile,
                    facility.CompanyId,
                }
            ).FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"File '{fileId}' was not found.");

            var tenant = await TenantAccessResolver.ResolveAsync(principal, dbContext, cancellationToken);
            tenant.EnsureCompany(file.CompanyId);

            await fileStorageService.DeleteAsync(file.PanelFile.StoragePath, cancellationToken);

            dbContext.PanelFiles.Remove(file.PanelFile);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .RequireAuthorization(AuthorizationPolicies.DeleteFiles)
        .WithName("DeletePanelFile");

        return app;
    }

    private static async Task<string> ParseCategoryAsync(
        PanoLoggerDbContext dbContext,
        string? category,
        CancellationToken cancellationToken)
    {
        var requestedCategory = category?.Trim();

        if (!string.IsNullOrWhiteSpace(requestedCategory)
            && await dbContext.PanelFileCategories.AnyAsync(item => item.Key == requestedCategory && item.IsActive, cancellationToken))
        {
            return requestedCategory;
        }

        var defaultCategory = await dbContext.PanelFileCategories
            .AsNoTracking()
            .Where(item => item.IsActive)
            .OrderBy(item => item.SortOrder)
            .Select(item => item.Key)
            .FirstOrDefaultAsync(cancellationToken);

        return defaultCategory ?? "PanelDocument";
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
