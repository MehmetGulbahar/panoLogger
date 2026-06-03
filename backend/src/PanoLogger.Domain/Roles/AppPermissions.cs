namespace PanoLogger.Domain.Roles;

public static class AppPermissions
{
    public const string ViewCompanies = "companies.view";
    public const string ViewFacilities = "facilities.view";
    public const string ViewPanels = "panels.view";
    public const string ViewFiles = "files.view";
    public const string ManageCompanies = "companies.manage";
    public const string ManageFacilities = "facilities.manage";
    public const string ManagePanels = "panels.manage";
    public const string ManageFiles = "files.manage";
    public const string ManageQrCodes = "qr.manage";
    public const string ViewReports = "reports.view";
    public const string ViewPublicPanels = "public-panels.view";

    public static IReadOnlyCollection<string> ForRoles(IEnumerable<string> roles)
    {
        var roleSet = roles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (roleSet.Contains(AppRoles.SuperAdmin))
        {
            permissions.UnionWith([
                ViewCompanies,
                ViewFacilities,
                ViewPanels,
                ViewFiles,
                ManageCompanies,
                ManageFacilities,
                ManagePanels,
                ManageFiles,
                ManageQrCodes,
                ViewReports,
                ViewPublicPanels
            ]);
        }

        if (roleSet.Contains(AppRoles.CompanyAdmin))
        {
            permissions.UnionWith([
                ViewCompanies,
                ViewFacilities,
                ViewPanels,
                ViewFiles,
                ManageFacilities,
                ManagePanels,
                ManageFiles,
                ManageQrCodes,
                ViewReports,
                ViewPublicPanels
            ]);
        }

        if (roleSet.Contains(AppRoles.FacilityManager))
        {
            permissions.UnionWith([
                ViewCompanies,
                ViewFacilities,
                ViewPanels,
                ViewFiles,
                ManagePanels,
                ManageFiles,
                ManageQrCodes,
                ViewReports,
                ViewPublicPanels
            ]);
        }

        if (roleSet.Contains(AppRoles.Viewer))
        {
            permissions.UnionWith([
                ViewCompanies,
                ViewFacilities,
                ViewPanels,
                ViewFiles,
                ViewReports,
                ViewPublicPanels
            ]);
        }

        return permissions.Order(StringComparer.OrdinalIgnoreCase).ToArray();
    }
}
