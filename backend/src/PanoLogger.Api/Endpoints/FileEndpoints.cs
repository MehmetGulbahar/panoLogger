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
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var totalPanels = await dbContext.Panels.CountAsync(cancellationToken);
            var totalFiles = await dbContext.PanelFiles.CountAsync(cancellationToken);
            var panelsWithFiles = await dbContext.PanelFiles
                .Select(file => file.PanelId)
                .Distinct()
                .CountAsync(cancellationToken);

            return Results.Ok(new FileSummaryResponse(totalFiles, panelsWithFiles, totalPanels));
        })
        .WithName("GetFileSummary");

        group.MapGet("/panels/{panelId:guid}", async (
            Guid panelId,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!await dbContext.Panels.AnyAsync(panel => panel.Id == panelId, cancellationToken))
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
            PanoLoggerDbContext dbContext,
            IFileStorageService fileStorageService,
            CancellationToken cancellationToken) =>
        {
            if (!await dbContext.Panels.AnyAsync(panel => panel.Id == panelId, cancellationToken))
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

        return app;
    }

    private static PanelFileCategory ParseCategory(string? category)
    {
        return Enum.TryParse<PanelFileCategory>(category, true, out var parsedCategory)
            ? parsedCategory
            : PanelFileCategory.PanelDocument;
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
