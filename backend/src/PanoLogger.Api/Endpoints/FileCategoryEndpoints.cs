using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PanoLogger.Api.Authorization;
using PanoLogger.Application.Common.Exceptions;
using PanoLogger.Domain.Files;
using PanoLogger.Infrastructure.Persistence;

namespace PanoLogger.Api.Endpoints;

public static class FileCategoryEndpoints
{
    public static IEndpointRouteBuilder MapFileCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/file-categories")
            .WithTags("Panel File Categories")
            .RequireAuthorization();

        group.MapGet("/", async (PanoLoggerDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var categories = await dbContext.PanelFileCategories
                .AsNoTracking()
                .Where(category => category.IsActive)
                .OrderBy(category => category.SortOrder)
                .ThenBy(category => category.Name)
                .Select(category => new FileCategoryResponse(
                    category.Id,
                    category.Key,
                    category.Name,
                    category.Description,
                    category.Icon,
                    category.SortOrder,
                    category.IsSystem))
                .ToArrayAsync(cancellationToken);

            return Results.Ok(categories);
        })
        .WithName("GetFileCategories");

        group.MapPost("/", async (
            UpsertFileCategoryRequest request,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var name = NormalizeName(request.Name);
            var key = NormalizeKey(request.Key, name);
            var description = NormalizeDescription(request.Description);
            var icon = NormalizeIcon(request.Icon);
            var sortOrder = request.SortOrder ?? await GetNextSortOrderAsync(dbContext, cancellationToken);

            if (await dbContext.PanelFileCategories.AnyAsync(category => category.Key == key, cancellationToken))
            {
                throw new ValidationException("Bu kategori anahtarı zaten kullanılıyor.");
            }

            var category = new PanelFileCategory
            {
                Key = key,
                Name = name,
                Description = description,
                Icon = icon,
                SortOrder = sortOrder,
                IsSystem = false,
                IsActive = true,
            };

            dbContext.PanelFileCategories.Add(category);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/file-categories/{category.Id}", ToResponse(category));
        })
        .RequireAuthorization(AuthorizationPolicies.ManageFiles)
        .WithName("CreateFileCategory");

        group.MapPut("/{categoryId:guid}", async (
            Guid categoryId,
            UpsertFileCategoryRequest request,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var category = await dbContext.PanelFileCategories.FirstOrDefaultAsync(item => item.Id == categoryId, cancellationToken)
                ?? throw new NotFoundException("Kategori bulunamadı.");

            category.Name = NormalizeName(request.Name);
            category.Description = NormalizeDescription(request.Description);
            category.Icon = NormalizeIcon(request.Icon);
            category.SortOrder = request.SortOrder ?? category.SortOrder;
            category.IsActive = true;
            category.MarkUpdated(DateTimeOffset.UtcNow);

            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(ToResponse(category));
        })
        .RequireAuthorization(AuthorizationPolicies.ManageFiles)
        .WithName("UpdateFileCategory");

        group.MapDelete("/{categoryId:guid}", async (
            Guid categoryId,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var category = await dbContext.PanelFileCategories.FirstOrDefaultAsync(item => item.Id == categoryId, cancellationToken)
                ?? throw new NotFoundException("Kategori bulunamadı.");

            var fileCount = await dbContext.PanelFiles.CountAsync(file => file.Category == category.Key, cancellationToken);
            if (fileCount > 0)
            {
                throw new ValidationException("Bu kategoride dosya var. Önce dosyaları taşıyın veya silin.");
            }

            dbContext.PanelFileCategories.Remove(category);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .RequireAuthorization(AuthorizationPolicies.ManageFiles)
        .WithName("DeleteFileCategory");

        return app;
    }

    private static FileCategoryResponse ToResponse(PanelFileCategory category)
    {
        return new FileCategoryResponse(
            category.Id,
            category.Key,
            category.Name,
            category.Description,
            category.Icon,
            category.SortOrder,
            category.IsSystem);
    }

    private static async Task<int> GetNextSortOrderAsync(PanoLoggerDbContext dbContext, CancellationToken cancellationToken)
    {
        var maxSortOrder = await dbContext.PanelFileCategories
            .Select(category => (int?)category.SortOrder)
            .MaxAsync(cancellationToken);

        return (maxSortOrder ?? 0) + 10;
    }

    private static string NormalizeName(string? name)
    {
        var normalizedName = name?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ValidationException("Kategori adı zorunludur.");
        }

        return normalizedName.Length <= 120 ? normalizedName : normalizedName[..120];
    }

    private static string NormalizeDescription(string? description)
    {
        var normalizedDescription = string.IsNullOrWhiteSpace(description)
            ? "Panel dosyaları"
            : description.Trim();

        return normalizedDescription.Length <= 240 ? normalizedDescription : normalizedDescription[..240];
    }

    private static string NormalizeIcon(string? icon)
    {
        var normalizedIcon = string.IsNullOrWhiteSpace(icon) ? "pi pi-file" : icon.Trim();
        return normalizedIcon.Length <= 80 ? normalizedIcon : normalizedIcon[..80];
    }

    private static string NormalizeKey(string? key, string fallbackName)
    {
        var source = string.IsNullOrWhiteSpace(key) ? fallbackName : key;
        var normalized = source.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
            if (unicodeCategory == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(character))
            {
                builder.Append(char.ToLowerInvariant(character));
            }
            else if (builder.Length > 0 && builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        var normalizedKey = builder.ToString().Trim('-');
        if (string.IsNullOrWhiteSpace(normalizedKey))
        {
            throw new ValidationException("Kategori anahtarı oluşturulamadı.");
        }

        return normalizedKey.Length <= 80 ? normalizedKey : normalizedKey[..80].Trim('-');
    }
}

public sealed record FileCategoryResponse(
    Guid Id,
    string Key,
    string Name,
    string Description,
    string Icon,
    int SortOrder,
    bool IsSystem);

public sealed record UpsertFileCategoryRequest(
    string Name,
    string? Key,
    string? Description,
    string? Icon,
    int? SortOrder);
