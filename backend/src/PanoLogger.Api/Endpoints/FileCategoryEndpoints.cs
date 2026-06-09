using System.Globalization;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PanoLogger.Api.Authorization;
using PanoLogger.Application.Common.Exceptions;
using PanoLogger.Domain.Files;
using PanoLogger.Infrastructure.Persistence;

namespace PanoLogger.Api.Endpoints;

public static class FileCategoryEndpoints
{
    private static readonly DefaultFileCategory[] DefaultCategories =
    [
        new("MaintenanceReport", "Bakım", "Periyodik bakım kayıtları", "pi pi-wrench", 10),
        new("ElectricalProject", "Tek Hat", "Tek hat ve elektrik proje dosyaları", "pi pi-sitemap", 20),
        new("PanelDocument", "Proje", "Yüklenen teknik proje dosyaları", "pi pi-file", 30),
    ];

    public static IEndpointRouteBuilder MapFileCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/file-categories")
            .WithTags("Panel File Categories")
            .RequireAuthorization();

        group.MapGet("/panels/{panelId:guid}", async (
            Guid panelId,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            await EnsurePanelAccessAsync(panelId, principal, dbContext, cancellationToken);
            await EnsureDefaultCategoriesAsync(panelId, dbContext, cancellationToken);

            var categories = await dbContext.PanelFileCategories
                .AsNoTracking()
                .Where(category => category.PanelId == panelId && category.IsActive)
                .OrderBy(category => category.SortOrder)
                .ThenBy(category => category.Name)
                .Select(category => new FileCategoryResponse(
                    category.Id,
                    category.PanelId,
                    category.Key,
                    category.Name,
                    category.Description,
                    category.Icon,
                    category.SortOrder,
                    category.IsSystem))
                .ToArrayAsync(cancellationToken);

            return Results.Ok(categories
                .GroupBy(category => category.Key, StringComparer.Ordinal)
                .Select(group => group.First())
                .ToArray());
        })
        .WithName("GetPanelFileCategories");

        group.MapPost("/panels/{panelId:guid}", async (
            Guid panelId,
            UpsertFileCategoryRequest request,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            await EnsurePanelAccessAsync(panelId, principal, dbContext, cancellationToken);
            await EnsureDefaultCategoriesAsync(panelId, dbContext, cancellationToken);

            var name = NormalizeName(request.Name);
            var key = NormalizeKey(request.Key, name);
            var description = NormalizeDescription(request.Description, name);
            var icon = NormalizeIcon(request.Icon);
            var sortOrder = request.SortOrder ?? await GetNextSortOrderAsync(panelId, dbContext, cancellationToken);

            if (await dbContext.PanelFileCategories.AnyAsync(
                category => category.PanelId == panelId && category.Key == key,
                cancellationToken))
            {
                throw new ValidationException("Bu panelde aynı kategori zaten var.");
            }

            var category = new PanelFileCategory
            {
                PanelId = panelId,
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
        .WithName("CreatePanelFileCategory");

        group.MapPut("/{categoryId:guid}", async (
            Guid categoryId,
            UpsertFileCategoryRequest request,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var category = await dbContext.PanelFileCategories.FirstOrDefaultAsync(item => item.Id == categoryId, cancellationToken)
                ?? throw new NotFoundException("Kategori bulunamadı.");
            await EnsurePanelAccessAsync(category.PanelId, principal, dbContext, cancellationToken);

            category.Name = NormalizeName(request.Name);
            category.Description = NormalizeDescription(request.Description, category.Name);
            category.Icon = NormalizeIcon(request.Icon);
            category.SortOrder = request.SortOrder ?? category.SortOrder;
            category.IsActive = true;
            category.MarkUpdated(DateTimeOffset.UtcNow);

            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(ToResponse(category));
        })
        .RequireAuthorization(AuthorizationPolicies.ManageFiles)
        .WithName("UpdatePanelFileCategory");

        group.MapDelete("/{categoryId:guid}", async (
            Guid categoryId,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var category = await dbContext.PanelFileCategories.FirstOrDefaultAsync(item => item.Id == categoryId, cancellationToken)
                ?? throw new NotFoundException("Kategori bulunamadı.");
            await EnsurePanelAccessAsync(category.PanelId, principal, dbContext, cancellationToken);

            var fileCount = await dbContext.PanelFiles.CountAsync(
                file => file.PanelId == category.PanelId && file.Category == category.Key,
                cancellationToken);
            if (fileCount > 0)
            {
                throw new ValidationException("Bu kategoride dosya var. Önce dosyaları taşıyın veya silin.");
            }

            dbContext.PanelFileCategories.Remove(category);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .RequireAuthorization(AuthorizationPolicies.ManageFiles)
        .WithName("DeletePanelFileCategory");

        return app;
    }

    public static async Task EnsureDefaultCategoriesAsync(
        Guid panelId,
        PanoLoggerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var existingKeys = await dbContext.PanelFileCategories
            .Where(category => category.PanelId == panelId)
            .Select(category => category.Key)
            .ToArrayAsync(cancellationToken);
        var existingKeySet = existingKeys.ToHashSet(StringComparer.Ordinal);

        foreach (var defaultCategory in DefaultCategories)
        {
            if (existingKeySet.Contains(defaultCategory.Key))
            {
                continue;
            }

            dbContext.PanelFileCategories.Add(new PanelFileCategory
            {
                PanelId = panelId,
                Key = defaultCategory.Key,
                Name = defaultCategory.Name,
                Description = defaultCategory.Description,
                Icon = defaultCategory.Icon,
                SortOrder = defaultCategory.SortOrder,
                IsSystem = true,
                IsActive = true,
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsurePanelAccessAsync(
        Guid panelId,
        ClaimsPrincipal principal,
        PanoLoggerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var panel = await (
            from panelItem in dbContext.Panels.AsNoTracking()
            join facility in dbContext.Facilities.AsNoTracking() on panelItem.FacilityId equals facility.Id
            where panelItem.Id == panelId
            select new { facility.CompanyId }
        ).FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException($"Panel '{panelId}' was not found.");

        var tenant = await TenantAccessResolver.ResolveAsync(principal, dbContext, cancellationToken);
        tenant.EnsureCompany(panel.CompanyId);
    }

    private static FileCategoryResponse ToResponse(PanelFileCategory category)
    {
        return new FileCategoryResponse(
            category.Id,
            category.PanelId,
            category.Key,
            category.Name,
            category.Description,
            category.Icon,
            category.SortOrder,
            category.IsSystem);
    }

    private static async Task<int> GetNextSortOrderAsync(
        Guid panelId,
        PanoLoggerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var maxSortOrder = await dbContext.PanelFileCategories
            .Where(category => category.PanelId == panelId)
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

    private static string NormalizeDescription(string? description, string categoryName)
    {
        var normalizedDescription = string.IsNullOrWhiteSpace(description)
            ? $"{categoryName} dosyaları"
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

    private sealed record DefaultFileCategory(string Key, string Name, string Description, string Icon, int SortOrder);
}

public sealed record FileCategoryResponse(
    Guid Id,
    Guid PanelId,
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
