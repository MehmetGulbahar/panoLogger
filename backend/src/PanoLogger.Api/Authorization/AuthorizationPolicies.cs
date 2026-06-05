using PanoLogger.Domain.Roles;

namespace PanoLogger.Api.Authorization;

public static class AuthorizationPolicies
{
    public const string ManageCompanies = nameof(ManageCompanies);
    public const string ManageFacilities = nameof(ManageFacilities);
    public const string ManagePanels = nameof(ManagePanels);
    public const string ManageFiles = nameof(ManageFiles);
    public const string DeleteFiles = nameof(DeleteFiles);
    public const string ManageQrCodes = nameof(ManageQrCodes);
    public const string ViewReports = nameof(ViewReports);

    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(ManageCompanies, policy => policy.RequireClaim("permission", AppPermissions.ManageCompanies));
            options.AddPolicy(ManageFacilities, policy => policy.RequireClaim("permission", AppPermissions.ManageFacilities));
            options.AddPolicy(ManagePanels, policy => policy.RequireClaim("permission", AppPermissions.ManagePanels));
            options.AddPolicy(ManageFiles, policy => policy.RequireClaim("permission", AppPermissions.ManageFiles));
            options.AddPolicy(DeleteFiles, policy => policy.RequireAssertion(context =>
                context.User.IsInRole(AppRoles.SuperAdmin)
                || context.User.HasClaim("permission", AppPermissions.DeleteFiles)));
            options.AddPolicy(ManageQrCodes, policy => policy.RequireClaim("permission", AppPermissions.ManageQrCodes));
            options.AddPolicy(ViewReports, policy => policy.RequireClaim("permission", AppPermissions.ViewReports));
        });

        return services;
    }
}
