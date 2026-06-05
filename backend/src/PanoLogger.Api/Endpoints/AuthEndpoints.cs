using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PanoLogger.Api.Authentication;
using PanoLogger.Application.Common.Exceptions;
using PanoLogger.Application.Common.Interfaces;
using PanoLogger.Domain.Roles;
using PanoLogger.Domain.Users;
using PanoLogger.Infrastructure.Authentication;
using PanoLogger.Infrastructure.Persistence;

namespace PanoLogger.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (
            RegisterRequest request,
            PanoLoggerDbContext dbContext,
            IPasswordHasher<User> passwordHasher,
            IJwtTokenService jwtTokenService,
            IOptions<JwtOptions> jwtOptions,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            ValidateRegistration(request);
            var email = request.Email.Trim().ToLowerInvariant();
            var companyCode = NormalizeCompanyCode(request.CompanyCode);

            if (await dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken))
            {
                throw new ValidationException("An account with this email already exists.");
            }

            var company = await dbContext.Companies
                .AsNoTracking()
                .Where(item => item.CompanyCode == companyCode)
                .Select(item => new { item.Id, item.CompanyCode })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new ValidationException("No company was found with this company code.");

            var viewerRoleId = await dbContext.Roles
                .Where(role => role.Name == AppRoles.Viewer)
                .Select(role => role.Id)
                .SingleAsync(cancellationToken);

            var user = new User
            {
                CompanyId = company.Id,
                Email = email,
                DisplayName = request.DisplayName.Trim(),
                IsActive = true,
            };
            user.SetPasswordHash(passwordHasher.HashPassword(user, request.Password));

            dbContext.Users.Add(user);
            dbContext.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = viewerRoleId,
            });

            await dbContext.SaveChangesAsync(cancellationToken);

            var permissions = await GetPermissionsForRolesAsync(dbContext, [AppRoles.Viewer], cancellationToken);

            return Results.Ok(CreateSessionResponse(
                user.Id,
                user.Email,
                user.DisplayName,
                user.CompanyId,
                [AppRoles.Viewer],
                permissions,
                jwtTokenService,
                jwtOptions.Value,
                httpContext));
        })
        .WithName("RegisterUser");

        group.MapPost("/login", async (
            LoginRequest request,
            PanoLoggerDbContext dbContext,
            IPasswordHasher<User> passwordHasher,
            IJwtTokenService jwtTokenService,
            IOptions<JwtOptions> jwtOptions,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await dbContext.Users.FirstOrDefaultAsync(item => item.Email == email, cancellationToken);

            if (user is null
                || !user.IsActive
                || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var roles = await (
                from userRole in dbContext.UserRoles
                join role in dbContext.Roles on userRole.RoleId equals role.Id
                where userRole.UserId == user.Id
                select role.Name
            ).ToArrayAsync(cancellationToken);
            roles = roles.Length == 0 ? [AppRoles.Viewer] : roles;

            if (!roles.Contains(AppRoles.SuperAdmin, StringComparer.OrdinalIgnoreCase) && user.CompanyId is null)
            {
                throw new UnauthorizedAccessException("User account is not linked to a company.");
            }

            var permissions = await GetPermissionsForRolesAsync(dbContext, roles, cancellationToken);

            return Results.Ok(CreateSessionResponse(
                user.Id,
                user.Email,
                user.DisplayName,
                user.CompanyId,
                roles,
                permissions,
                jwtTokenService,
                jwtOptions.Value,
                httpContext));
        })
        .WithName("LoginUser");

        group.MapPost("/dev-token", (
            DevTokenRequest request,
            IJwtTokenService jwtTokenService,
            IOptions<JwtOptions> jwtOptions,
            PanoLoggerDbContext dbContext,
            HttpContext httpContext) =>
        {
            var userId = request.UserId ?? Guid.NewGuid();
            var email = string.IsNullOrWhiteSpace(request.Email) ? "demo@panologger.local" : request.Email;
            var displayName = email.Split('@', 2)[0];
            var roles = NormalizeRoles(request.Roles);
            var permissions = AppPermissions.ForRoles(roles);

            return Results.Ok(CreateSessionResponse(userId, email, displayName, null, roles, permissions, jwtTokenService, jwtOptions.Value, httpContext));
        })
        .WithName("CreateDevelopmentToken");

        group.MapPost("/logout", (HttpContext httpContext) =>
        {
            httpContext.Response.Cookies.Delete(JwtAuthenticationExtensions.AccessTokenCookieName, CreateCookieOptions(httpContext));
            return Results.NoContent();
        })
        .WithName("LogoutUser");

        group.MapGet("/me", async (
            ClaimsPrincipal principal,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await dbContext.Users
                .AsNoTracking()
                .Where(item => item.Id == userId && item.IsActive)
                .Select(item => new { item.Id, item.Email, item.DisplayName, item.CompanyId })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new UnauthorizedAccessException("User account was not found or is inactive.");

            return Results.Ok(new
            {
                id = user.Id,
                email = user.Email,
                displayName = user.DisplayName,
                companyId = user.CompanyId,
                roles = principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray(),
            });
        })
        .RequireAuthorization()
        .WithName("GetCurrentUser");

        group.MapGet("/permissions", async (
            ClaimsPrincipal user,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var roles = await GetUserRolesAsync(user, dbContext, cancellationToken);
            var permissions = await GetPermissionsForRolesAsync(dbContext, roles, cancellationToken);

            return Results.Ok(new
            {
                roles,
                permissions,
            });
        })
        .RequireAuthorization()
        .WithName("GetCurrentUserPermissions");

        return app;
    }

    private static object CreateSessionResponse(
        Guid userId,
        string email,
        string displayName,
        Guid? companyId,
        string[] roles,
        IReadOnlyCollection<string> permissions,
        IJwtTokenService jwtTokenService,
        JwtOptions jwtOptions,
        HttpContext httpContext)
    {
        var accessToken = jwtTokenService.CreateAccessToken(userId, email, companyId, roles, permissions);
        httpContext.Response.Cookies.Append(
            JwtAuthenticationExtensions.AccessTokenCookieName,
            accessToken,
            CreateCookieOptions(httpContext, DateTimeOffset.UtcNow.AddMinutes(jwtOptions.AccessTokenMinutes)));

        return new
        {
            user = new
            {
                id = userId,
                email,
                displayName,
                companyId,
                roles,
                permissions,
            },
        };
    }

    private static async Task<string[]> GetUserRolesAsync(
        ClaimsPrincipal principal,
        PanoLoggerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var roles = await (
            from userRole in dbContext.UserRoles.AsNoTracking()
            join role in dbContext.Roles.AsNoTracking() on userRole.RoleId equals role.Id
            where userRole.UserId == userId
            orderby role.Name
            select role.Name
        ).ToArrayAsync(cancellationToken);

        return roles.Length == 0
            ? principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray()
            : roles;
    }

    private static async Task<string[]> GetPermissionsForRolesAsync(
        PanoLoggerDbContext dbContext,
        string[] roles,
        CancellationToken cancellationToken)
    {
        var permissions = await (
            from role in dbContext.Roles.AsNoTracking()
            join rolePermission in dbContext.RolePermissions.AsNoTracking() on role.Id equals rolePermission.RoleId
            where roles.Contains(role.Name)
            select rolePermission.Permission
        ).Distinct().Order().ToArrayAsync(cancellationToken);

        return permissions.Length == 0 ? AppPermissions.ForRoles(roles).ToArray() : permissions;
    }

    private static CookieOptions CreateCookieOptions(HttpContext httpContext, DateTimeOffset? expires = null)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = expires,
        };
    }

    private static void ValidateRegistration(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Trim().Length < 2)
        {
            throw new ValidationException("Display name must contain at least 2 characters.");
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@', StringComparison.Ordinal))
        {
            throw new ValidationException("A valid email address is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            throw new ValidationException("Password must contain at least 8 characters.");
        }

        _ = NormalizeCompanyCode(request.CompanyCode);
    }

    private static string NormalizeCompanyCode(string? companyCode)
    {
        var normalized = companyCode?.Trim().ToUpperInvariant() ?? "";
        if (normalized.Length == 0)
        {
            throw new ValidationException("Company code is required.");
        }

        return normalized;
    }

    private static string[] NormalizeRoles(string[]? roles)
    {
        if (roles is not { Length: > 0 })
        {
            return [AppRoles.SuperAdmin];
        }

        var requestedRoles = roles
            .Where(role => AppRoles.All.Contains(role, StringComparer.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return requestedRoles.Length == 0 ? [AppRoles.Viewer] : requestedRoles;
    }
}

public sealed record RegisterRequest(string DisplayName, string Email, string Password, string CompanyCode);

public sealed record LoginRequest(string Email, string Password);

public sealed record DevTokenRequest(Guid? UserId, string? Email, string[]? Roles);
