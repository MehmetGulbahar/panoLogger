using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PanoLogger.Api.Authorization;
using PanoLogger.Application.Common.Exceptions;
using PanoLogger.Application.Common.Interfaces;
using PanoLogger.Domain.Roles;
using PanoLogger.Domain.Users;
using PanoLogger.Infrastructure.Persistence;

namespace PanoLogger.Api.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin")
            .WithTags("Admin")
            .RequireAuthorization(policy => policy.RequireRole(AppRoles.SuperAdmin));

        group.MapGet("/overview", async (PanoLoggerDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var companies = await dbContext.Companies.CountAsync(cancellationToken);
            var facilities = await dbContext.Facilities.CountAsync(cancellationToken);
            var panels = await dbContext.Panels.CountAsync(cancellationToken);
            var files = await dbContext.PanelFiles.CountAsync(cancellationToken);
            var users = await dbContext.Users.CountAsync(cancellationToken);
            var activeUsers = await dbContext.Users.CountAsync(user => user.IsActive, cancellationToken);
            var roles = await dbContext.Roles.CountAsync(cancellationToken);

            return Results.Ok(new AdminOverviewResponse(
                companies,
                facilities,
                panels,
                files,
                users,
                activeUsers,
                users - activeUsers,
                roles));
        })
        .WithName("GetAdminOverview");

        group.MapGet("/companies/options", async (PanoLoggerDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var companies = await dbContext.Companies
                .AsNoTracking()
                .OrderBy(company => company.ProjectName)
                .Select(company => new AdminCompanyOptionResponse(
                    company.Id,
                    company.ProjectName,
                    company.Name,
                    company.CompanyCode))
                .ToArrayAsync(cancellationToken);

            return Results.Ok(companies);
        })
        .WithName("GetAdminCompanyOptions");

        group.MapGet("/roles", async (PanoLoggerDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var roles = await dbContext.Roles
                .AsNoTracking()
                .OrderBy(role => role.Name)
                .ToArrayAsync(cancellationToken);
            var roleIds = roles.Select(role => role.Id).ToArray();
            var rolePermissions = await dbContext.RolePermissions
                .AsNoTracking()
                .Where(item => roleIds.Contains(item.RoleId))
                .ToArrayAsync(cancellationToken);
            var permissionsByRole = rolePermissions
                .GroupBy(item => item.RoleId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(item => item.Permission).Order(StringComparer.OrdinalIgnoreCase).ToArray());

            return Results.Ok(roles.Select(role => new AdminRoleResponse(
                role.Id,
                role.Name,
                role.Description,
                permissionsByRole.GetValueOrDefault(role.Id, []),
                AppRoles.All.Contains(role.Name, StringComparer.OrdinalIgnoreCase))));
        })
        .WithName("GetAdminRoles");

        group.MapGet("/permissions", () => Results.Ok(AppPermissions.All.Order(StringComparer.OrdinalIgnoreCase).ToArray()))
            .WithName("GetAdminAvailablePermissions");

        group.MapGet("/audit-logs", async (PanoLoggerDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var logs = await (
                from auditLog in dbContext.AuditLogs.AsNoTracking()
                join user in dbContext.Users.AsNoTracking() on auditLog.UserId equals user.Id into userJoin
                from user in userJoin.DefaultIfEmpty()
                join directPanel in dbContext.Panels.AsNoTracking() on auditLog.EntityId equals directPanel.Id into directPanelJoin
                from directPanel in directPanelJoin.DefaultIfEmpty()
                join panelFile in dbContext.PanelFiles.AsNoTracking() on auditLog.EntityId equals panelFile.Id into panelFileJoin
                from panelFile in panelFileJoin.DefaultIfEmpty()
                join filePanel in dbContext.Panels.AsNoTracking() on panelFile.PanelId equals filePanel.Id into filePanelJoin
                from filePanel in filePanelJoin.DefaultIfEmpty()
                orderby auditLog.OccurredAtUtc descending
                select new AdminAuditLogResponse(
                    auditLog.Id,
                    auditLog.Action,
                    auditLog.EntityName,
                    auditLog.EntityId,
                    auditLog.UserId,
                    user == null ? null : user.Username,
                    string.Equals(auditLog.EntityName, "Panel", StringComparison.OrdinalIgnoreCase)
                        ? (directPanel == null ? null : directPanel.Name)
                        : string.Equals(auditLog.EntityName, "PanelFile", StringComparison.OrdinalIgnoreCase)
                            ? (filePanel == null ? null : filePanel.Name)
                            : null,
                    auditLog.OccurredAtUtc,
                    auditLog.Metadata))
                .Take(100)
                .ToArrayAsync(cancellationToken);

            return Results.Ok(logs);
        })
        .WithName("GetAdminAuditLogs");

        group.MapGet("/maintenance-reports", async (PanoLoggerDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var reports = await (
                from report in dbContext.MaintenanceReports.AsNoTracking()
                join panel in dbContext.Panels.AsNoTracking() on report.PanelId equals panel.Id
                join facility in dbContext.Facilities.AsNoTracking() on panel.FacilityId equals facility.Id
                join company in dbContext.Companies.AsNoTracking() on facility.CompanyId equals company.Id
                join user in dbContext.Users.AsNoTracking() on report.CreatedByUserId equals user.Id into userJoin
                from user in userJoin.DefaultIfEmpty()
                orderby report.ReportDateUtc descending
                select new AdminMaintenanceReportResponse(
                    report.Id,
                    report.PanelId,
                    panel.Name,
                    panel.Code,
                    facility.Name,
                    company.ProjectName,
                    report.Title,
                    report.ReportDateUtc,
                    report.Notes,
                    user == null ? null : user.Username,
                    report.CreatedAtUtc))
                .Take(100)
                .ToArrayAsync(cancellationToken);
           
            
            return Results.Ok(reports);
        })
        .WithName("GetAdminMaintenanceReports");

        group.MapPost("/users", async (
            CreateAdminUserRequest request,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IPasswordHasher<User> passwordHasher,
            IAuditLogWriter auditLogWriter,
            CancellationToken cancellationToken) =>
        {
            var username = NormalizeUsername(request.Username);
            var displayName = NormalizeDisplayName(request.DisplayName);
            var password = NormalizePassword(request.Password);
            var roleEntities = await dbContext.Roles
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
            var rolesByName = roleEntities.ToDictionary(role => role.Name, StringComparer.OrdinalIgnoreCase);
            var requestedRoles = NormalizeUserRoles(request.Roles, rolesByName.Keys);

            if (await dbContext.Users.AnyAsync(user => user.Username == username, cancellationToken))
            {
                throw new ValidationException("A user with this username already exists.");
            }

            if (request.CompanyId is not null
                && !await dbContext.Companies.AnyAsync(company => company.Id == request.CompanyId, cancellationToken))
            {
                throw new ValidationException("Selected company was not found.");
            }

            if (!requestedRoles.Contains(AppRoles.SuperAdmin, StringComparer.OrdinalIgnoreCase) && request.CompanyId is null)
            {
                throw new ValidationException("Company is required for non-superadmin users.");
            }

            var user = new User
            {
                Username = username,
                DisplayName = displayName,
                CompanyId = request.CompanyId,
                IsActive = true,
            };
            user.SetPasswordHash(passwordHasher.HashPassword(user, password));

            dbContext.Users.Add(user);
            foreach (var roleName in requestedRoles)
            {
                dbContext.UserRoles.Add(new()
                {
                    UserId = user.Id,
                    RoleId = rolesByName[roleName].Id,
                });
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await auditLogWriter.WriteAsync(
                "users.create",
                nameof(User),
                user.Id.ToString(),
                GetCurrentUserId(principal),
                cancellationToken);

            return Results.Created($"/api/admin/users/{user.Id}", new AdminUserResponse(
                user.Id,
                user.Username,
                user.DisplayName,
                user.CompanyId,
                null,
                null,
                null,
                user.IsActive,
                requestedRoles,
                user.CreatedAtUtc));
        })
        .WithName("CreateAdminUser");

        group.MapPost("/roles", async (
            CreateAdminRoleRequest request,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IAuditLogWriter auditLogWriter,
            CancellationToken cancellationToken) =>
        {
            var name = NormalizeRoleName(request.Name);
            var description = NormalizeRoleDescription(request.Description);
            var permissions = NormalizePermissions(request.Permissions);

            if (AppRoles.All.Contains(name, StringComparer.OrdinalIgnoreCase)
                || await dbContext.Roles.AnyAsync(role => role.Name == name, cancellationToken))
            {
                throw new ValidationException("A role with this name already exists.");
            }

            var role = new Role
            {
                Name = name,
                Description = description,
            };

            dbContext.Roles.Add(role);
            AddRolePermissions(dbContext, role.Id, permissions);
            await dbContext.SaveChangesAsync(cancellationToken);
            await auditLogWriter.WriteAsync(
                "roles.create",
                nameof(Role),
                role.Id.ToString(),
                GetCurrentUserId(principal),
                cancellationToken);

            return Results.Created($"/api/admin/roles/{role.Id}", new AdminRoleResponse(
                role.Id,
                role.Name,
                role.Description,
                permissions,
                false));
        })
        .WithName("CreateAdminRole");

        group.MapPut("/roles/{roleId:guid}", async (
            Guid roleId,
            UpdateAdminRoleRequest request,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IAuditLogWriter auditLogWriter,
            CancellationToken cancellationToken) =>
        {
            var role = await dbContext.Roles
                .AsNoTracking()
                .Where(item => item.Id == roleId)
                .Select(item => new { item.Id, item.Name })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("Role was not found.");

            if (string.Equals(role.Name, AppRoles.SuperAdmin, StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException("The SuperAdmin role is protected and cannot be edited.");
            }

            var description = NormalizeRoleDescription(request.Description);
            var permissions = NormalizePermissions(request.Permissions);

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            await dbContext.Roles
                .Where(item => item.Id == roleId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(item => item.Description, description),
                    cancellationToken);

            await dbContext.RolePermissions
                .Where(item => item.RoleId == roleId)
                .ExecuteDeleteAsync(cancellationToken);

            AddRolePermissions(dbContext, roleId, permissions);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            await auditLogWriter.WriteAsync(
                "roles.update",
                nameof(Role),
                roleId.ToString(),
                GetCurrentUserId(principal),
                cancellationToken);

            return Results.NoContent();
        })
        .WithName("UpdateAdminRole");

        group.MapDelete("/roles/{roleId:guid}", async (
            Guid roleId,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IAuditLogWriter auditLogWriter,
            CancellationToken cancellationToken) =>
        {
            var role = await dbContext.Roles
                .AsNoTracking()
                .Where(item => item.Id == roleId)
                .Select(item => new { item.Id, item.Name })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("Role was not found.");

            if (AppRoles.All.Contains(role.Name, StringComparer.OrdinalIgnoreCase))
            {
                throw new ValidationException("Built-in system roles cannot be deleted.");
            }

            var assignedUsers = await dbContext.UserRoles
                .CountAsync(item => item.RoleId == roleId, cancellationToken);

            if (assignedUsers > 0)
            {
                throw new ValidationException("This role is assigned to users. Remove it from users before deleting it.");
            }

            await dbContext.Roles
                .Where(item => item.Id == roleId)
                .ExecuteDeleteAsync(cancellationToken);
            await auditLogWriter.WriteAsync(
                "roles.delete",
                nameof(Role),
                roleId.ToString(),
                GetCurrentUserId(principal),
                cancellationToken);

            return Results.NoContent();
        })
        .WithName("DeleteAdminRole");

        group.MapGet("/users", async (PanoLoggerDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var users = await (
                from user in dbContext.Users.AsNoTracking()
                join company in dbContext.Companies.AsNoTracking() on user.CompanyId equals company.Id into companyJoin
                from company in companyJoin.DefaultIfEmpty()
                orderby user.CreatedAtUtc descending
                select new
                {
                    user.Id,
                    user.Username,
                    user.DisplayName,
                    user.CompanyId,
                    CompanyName = company == null ? null : company.Name,
                    ProjectName = company == null ? null : company.ProjectName,
                    CompanyCode = company == null ? null : company.CompanyCode,
                    user.IsActive,
                    user.CreatedAtUtc,
                })
                .ToArrayAsync(cancellationToken);

            var roles = await (
                from userRole in dbContext.UserRoles.AsNoTracking()
                join role in dbContext.Roles.AsNoTracking() on userRole.RoleId equals role.Id
                select new { userRole.UserId, role.Name })
                .ToArrayAsync(cancellationToken);

            var rolesByUser = roles
                .GroupBy(item => item.UserId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(item => item.Name).Order(StringComparer.OrdinalIgnoreCase).ToArray());

            return Results.Ok(users.Select(user => new AdminUserResponse(
                user.Id,
                user.Username,
                user.DisplayName,
                user.CompanyId,
                user.CompanyName,
                user.ProjectName,
                user.CompanyCode,
                user.IsActive,
                rolesByUser.GetValueOrDefault(user.Id, [AppRoles.Viewer]),
                user.CreatedAtUtc)));
        })
        .WithName("GetAdminUsers");

        group.MapPut("/users/{userId:guid}", async (
            Guid userId,
            UpdateAdminUserRequest request,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IAuditLogWriter auditLogWriter,
            CancellationToken cancellationToken) =>
        {
            var currentUserId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var displayName = NormalizeDisplayName(request.DisplayName);
            var roleEntities = await dbContext.Roles
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
            var rolesByName = roleEntities.ToDictionary(role => role.Name, StringComparer.OrdinalIgnoreCase);
            var requestedRoles = NormalizeUserRoles(request.Roles, rolesByName.Keys);

            var user = await dbContext.Users
                .AsNoTracking()
                .Where(item => item.Id == userId)
                .Select(item => new { item.Id, item.CompanyId, item.IsActive })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("User was not found.");

            if (request.CompanyId is not null
                && !await dbContext.Companies.AnyAsync(company => company.Id == request.CompanyId, cancellationToken))
            {
                throw new ValidationException("Selected company was not found.");
            }

            if (!requestedRoles.Contains(AppRoles.SuperAdmin, StringComparer.OrdinalIgnoreCase) && request.CompanyId is null)
            {
                throw new ValidationException("Company is required for non-superadmin users.");
            }

            if (userId == currentUserId && !request.IsActive)
            {
                throw new ValidationException("You cannot deactivate your own SuperAdmin account.");
            }

            if (userId == currentUserId && !requestedRoles.Contains(AppRoles.SuperAdmin, StringComparer.OrdinalIgnoreCase))
            {
                throw new ValidationException("You cannot remove SuperAdmin from your own account.");
            }

            var superAdminRoleId = rolesByName[AppRoles.SuperAdmin].Id;

            var hasSuperAdminRole = await dbContext.UserRoles
                .AnyAsync(item => item.UserId == userId && item.RoleId == superAdminRoleId, cancellationToken);

            if (hasSuperAdminRole && !requestedRoles.Contains(AppRoles.SuperAdmin, StringComparer.OrdinalIgnoreCase))
            {
                var otherSuperAdmins = await dbContext.UserRoles
                    .CountAsync(item => item.UserId != userId && item.RoleId == superAdminRoleId, cancellationToken);

                if (otherSuperAdmins == 0)
                {
                    throw new ValidationException("At least one SuperAdmin account must remain active.");
                }
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            await dbContext.Users
                .Where(item => item.Id == userId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(item => item.DisplayName, displayName)
                    .SetProperty(item => item.CompanyId, request.CompanyId)
                    .SetProperty(item => item.IsActive, request.IsActive),
                    cancellationToken);

            await dbContext.UserRoles
                .Where(item => item.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);

            foreach (var roleName in requestedRoles)
            {
                dbContext.UserRoles.Add(new()
                {
                    UserId = userId,
                    RoleId = rolesByName[roleName].Id,
                });
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            await auditLogWriter.WriteAsync(
                "users.update",
                nameof(User),
                userId.ToString(),
                GetCurrentUserId(principal),
                cancellationToken);

            return Results.NoContent();
        })
        .WithName("UpdateAdminUser");

        group.MapPut("/users/{userId:guid}/password", async (
            Guid userId,
            ResetAdminUserPasswordRequest request,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IPasswordHasher<User> passwordHasher,
            IAuditLogWriter auditLogWriter,
            CancellationToken cancellationToken) =>
        {
            var password = NormalizePassword(request.Password);
            var user = await dbContext.Users
                .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken)
                ?? throw new NotFoundException("User was not found.");

            user.SetPasswordHash(passwordHasher.HashPassword(user, password));
            await dbContext.SaveChangesAsync(cancellationToken);
            await auditLogWriter.WriteAsync(
                "users.password.reset",
                nameof(User),
                userId.ToString(),
                GetCurrentUserId(principal),
                cancellationToken);

            return Results.NoContent();
        })
        .WithName("ResetAdminUserPassword");

        group.MapDelete("/users/{userId:guid}", async (
            Guid userId,
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            IAuditLogWriter auditLogWriter,
            CancellationToken cancellationToken) =>
        {
            var currentUserId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (userId == currentUserId)
            {
                throw new ValidationException("You cannot delete your own SuperAdmin account.");
            }

            var user = await dbContext.Users
                .AsNoTracking()
                .Where(item => item.Id == userId)
                .Select(item => new { item.Id, item.Username, item.DisplayName })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("User was not found.");

            var superAdminRoleId = await dbContext.Roles
                .Where(role => role.Name == AppRoles.SuperAdmin)
                .Select(role => role.Id)
                .SingleAsync(cancellationToken);

            var isSuperAdmin = await dbContext.UserRoles
                .AnyAsync(item => item.UserId == userId && item.RoleId == superAdminRoleId, cancellationToken);

            if (isSuperAdmin)
            {
                var otherSuperAdmins = await dbContext.UserRoles
                    .CountAsync(item => item.UserId != userId && item.RoleId == superAdminRoleId, cancellationToken);

                if (otherSuperAdmins == 0)
                {
                    throw new ValidationException("At least one SuperAdmin account must remain in the system.");
                }
            }

            await dbContext.Users
                .Where(item => item.Id == user.Id)
                .ExecuteDeleteAsync(cancellationToken);
            await auditLogWriter.WriteAsync(
                "users.delete",
                nameof(User),
                user.Id.ToString(),
                GetCurrentUserId(principal),
                cancellationToken);

            return Results.NoContent();
        })
        .WithName("DeleteAdminUser");

        return app;
    }

    private static string NormalizeDisplayName(string displayName)
    {
        var normalized = displayName.Trim();
        if (normalized.Length < 2)
        {
            throw new ValidationException("Display name must contain at least 2 characters.");
        }

        if (normalized.Length > 160)
        {
            throw new ValidationException("Display name must contain at most 160 characters.");
        }

        return normalized;
    }

    private static string NormalizeUsername(string username)
    {
        var normalized = username.Trim().ToLowerInvariant();
        if (normalized.Length < 3 || normalized.Length > 80)
        {
            throw new ValidationException("Username must contain between 3 and 80 characters.");
        }

        if (!normalized.All(character => char.IsLetterOrDigit(character) || character is '.' or '-' or '_'))
        {
            throw new ValidationException("Username can contain only letters, numbers, dot, dash, and underscore.");
        }

        return normalized;
    }

    private static string NormalizePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8 || password.Length > 128)
        {
            throw new ValidationException("Password must contain between 8 and 128 characters.");
        }

        return password;
    }

    private static string[] NormalizeUserRoles(string[] roles, IEnumerable<string> knownRoles)
    {
        if (roles.Length == 0)
        {
            throw new ValidationException("At least one role is required.");
        }

        var knownRoleSet = knownRoles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var normalized = roles
            .Select(role => knownRoleSet.FirstOrDefault(item => string.Equals(item, role, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        if (normalized.Any(role => role is null))
        {
            throw new ValidationException("One or more selected roles are invalid.");
        }

        return normalized
            .OfType<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string NormalizeRoleName(string name)
    {
        var normalized = name.Trim();
        if (normalized.Length < 2 || normalized.Length > 80)
        {
            throw new ValidationException("Role name must contain between 2 and 80 characters.");
        }

        if (!normalized.All(character => char.IsLetterOrDigit(character) || character is '-' or '_'))
        {
            throw new ValidationException("Role name can contain only letters, numbers, dash, and underscore.");
        }

        return normalized;
    }

    private static string NormalizeRoleDescription(string description)
    {
        var normalized = description.Trim();
        if (normalized.Length < 4 || normalized.Length > 300)
        {
            throw new ValidationException("Role description must contain between 4 and 300 characters.");
        }

        return normalized;
    }

    private static string[] NormalizePermissions(string[] permissions)
    {
        var allowedPermissions = AppPermissions.All.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var normalized = permissions
            .Select(permission => allowedPermissions.FirstOrDefault(item => string.Equals(item, permission, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        if (normalized.Any(permission => permission is null))
        {
            throw new ValidationException("One or more selected permissions are invalid.");
        }

        return normalized
            .OfType<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static void AddRolePermissions(PanoLoggerDbContext dbContext, Guid roleId, IEnumerable<string> permissions)
    {
        foreach (var permission in permissions)
        {
            dbContext.RolePermissions.Add(new()
            {
                RoleId = roleId,
                Permission = permission,
            });
        }
    }

    private static Guid? GetCurrentUserId(ClaimsPrincipal principal)
    {
        return Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : null;
    }
}

public sealed record AdminOverviewResponse(
    int Companies,
    int Facilities,
    int Panels,
    int Files,
    int Users,
    int ActiveUsers,
    int InactiveUsers,
    int Roles);

public sealed record AdminCompanyOptionResponse(
    Guid Id,
    string ProjectName,
    string Name,
    string CompanyCode);

public sealed record AdminRoleResponse(
    Guid Id,
    string Name,
    string Description,
    string[] Permissions,
    bool IsProtected);

public sealed record AdminUserResponse(
    Guid Id,
    string Username,
    string DisplayName,
    Guid? CompanyId,
    string? CompanyName,
    string? ProjectName,
    string? CompanyCode,
    bool IsActive,
    string[] Roles,
    DateTimeOffset CreatedAtUtc);

public sealed record AdminAuditLogResponse(
    Guid Id,
    string Action,
    string EntityName,
    Guid? EntityId,
    Guid? UserId,
    string? Username,
    string? PanelName,
    DateTimeOffset OccurredAtUtc,
    string Metadata);

public sealed record AdminMaintenanceReportResponse(
    Guid Id,
    Guid PanelId,
    string PanelName,
    string PanelCode,
    string FacilityName,
    string ProjectName,
    string Title,
    DateTimeOffset ReportDateUtc,
    string Notes,
    string? CreatedByUsername,
    DateTimeOffset CreatedAtUtc);

public sealed record CreateAdminUserRequest(
    string Username,
    string DisplayName,
    string Password,
    Guid? CompanyId,
    string[] Roles);

public sealed record UpdateAdminUserRequest(
    string DisplayName,
    Guid? CompanyId,
    bool IsActive,
    string[] Roles);

public sealed record ResetAdminUserPasswordRequest(string Password);

public sealed record CreateAdminRoleRequest(
    string Name,
    string Description,
    string[] Permissions);

public sealed record UpdateAdminRoleRequest(
    string Description,
    string[] Permissions);
