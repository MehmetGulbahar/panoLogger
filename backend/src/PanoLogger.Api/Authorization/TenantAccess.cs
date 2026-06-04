using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PanoLogger.Domain.Roles;
using PanoLogger.Infrastructure.Persistence;

namespace PanoLogger.Api.Authorization;

public sealed record TenantAccess(bool IsSuperAdmin, Guid? CompanyId)
{
    public bool CanAccessCompany(Guid companyId) => IsSuperAdmin || CompanyId == companyId;

    public void EnsureCompany(Guid companyId)
    {
        if (!CanAccessCompany(companyId))
        {
            throw new UnauthorizedAccessException("You are not allowed to access this company's data.");
        }
    }
}

public static class TenantAccessResolver
{
    public static async Task<TenantAccess> ResolveAsync(
        ClaimsPrincipal principal,
        PanoLoggerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var roles = principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray();
        var isSuperAdmin = roles.Contains(AppRoles.SuperAdmin, StringComparer.OrdinalIgnoreCase);

        if (isSuperAdmin)
        {
            return new TenantAccess(true, null);
        }

        var userIdValue = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User identity is missing.");

        var userId = Guid.Parse(userIdValue);
        var companyId = await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == userId && user.IsActive)
            .Select(user => user.CompanyId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new UnauthorizedAccessException("User account is not linked to a company.");

        return new TenantAccess(false, companyId);
    }
}
