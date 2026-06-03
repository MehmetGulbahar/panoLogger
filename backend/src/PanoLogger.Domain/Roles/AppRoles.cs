namespace PanoLogger.Domain.Roles;

public static class AppRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string CompanyAdmin = "CompanyAdmin";
    public const string FacilityManager = "FacilityManager";
    public const string Viewer = "Viewer";

    public static readonly string[] All =
    [
        SuperAdmin,
        CompanyAdmin,
        FacilityManager,
        Viewer
    ];
}
