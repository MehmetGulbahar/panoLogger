using PanoLogger.Domain.Roles;

namespace PanoLogger.Api.Authorization;

public static class AuthorizationPolicies
{
    public const string ManageCompanies = nameof(ManageCompanies);
    public const string ManageFacilities = nameof(ManageFacilities);
    public const string ManagePanels = nameof(ManagePanels);
    public const string ManageFiles = nameof(ManageFiles);
    public const string ManageQrCodes = nameof(ManageQrCodes);
    public const string ViewReports = nameof(ViewReports);

    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(ManageCompanies, policy => policy.RequireRole(AppRoles.SuperAdmin));
            options.AddPolicy(ManageFacilities, policy => policy.RequireRole(AppRoles.SuperAdmin, AppRoles.CompanyAdmin));
            options.AddPolicy(ManagePanels, policy => policy.RequireRole(AppRoles.SuperAdmin, AppRoles.CompanyAdmin, AppRoles.FacilityManager));
            options.AddPolicy(ManageFiles, policy => policy.RequireRole(AppRoles.SuperAdmin, AppRoles.CompanyAdmin, AppRoles.FacilityManager));
            options.AddPolicy(ManageQrCodes, policy => policy.RequireRole(AppRoles.SuperAdmin, AppRoles.CompanyAdmin, AppRoles.FacilityManager));
            options.AddPolicy(ViewReports, policy => policy.RequireRole(AppRoles.SuperAdmin, AppRoles.CompanyAdmin, AppRoles.FacilityManager, AppRoles.Viewer));
        });

        return services;
    }
}
