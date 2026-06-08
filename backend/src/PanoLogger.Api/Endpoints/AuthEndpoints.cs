using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PanoLogger.Api.Authentication;
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

        group.MapPost("/login", async (
            LoginRequest request,
            PanoLoggerDbContext dbContext,
            IPasswordHasher<User> passwordHasher,
            IJwtTokenService jwtTokenService,
            IOptions<JwtOptions> jwtOptions,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var username = NormalizeUsername(request.Username);
            var user = await dbContext.Users.FirstOrDefaultAsync(item => item.Username == username, cancellationToken);

            if (user is null
                || !user.IsActive
                || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
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
                user.Username,
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
            HttpContext httpContext) =>
        {
            var userId = request.UserId ?? Guid.NewGuid();
            var username = string.IsNullOrWhiteSpace(request.Username) ? "demo" : NormalizeUsername(request.Username);
            var displayName = username;
            var roles = NormalizeRoles(request.Roles);
            var permissions = AppPermissions.ForRoles(roles);

            return Results.Ok(CreateSessionResponse(userId, username, displayName, null, roles, permissions, jwtTokenService, jwtOptions.Value, httpContext));
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
                .Select(item => new { item.Id, item.Username, item.DisplayName, item.CompanyId })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new UnauthorizedAccessException("User account was not found or is inactive.");

            return Results.Ok(new
            {
                id = user.Id,
                username = user.Username,
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
        string username,
        string displayName,
        Guid? companyId,
        string[] roles,
        IReadOnlyCollection<string> permissions,
        IJwtTokenService jwtTokenService,
        JwtOptions jwtOptions,
        HttpContext httpContext)
    {
        var accessToken = jwtTokenService.CreateAccessToken(userId, username, companyId, roles, permissions);
        httpContext.Response.Cookies.Append(
            JwtAuthenticationExtensions.AccessTokenCookieName,
            accessToken,
            CreateCookieOptions(httpContext, DateTimeOffset.UtcNow.AddMinutes(jwtOptions.AccessTokenMinutes)));

        return new
        {
            user = new
            {
                id = userId,
                username,
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

    private static string NormalizeUsername(string username)
    {
        return username.Trim().ToLowerInvariant();
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

public sealed record LoginRequest(string Username, string Password);

public sealed record DevTokenRequest(Guid? UserId, string? Username, string[]? Roles);
