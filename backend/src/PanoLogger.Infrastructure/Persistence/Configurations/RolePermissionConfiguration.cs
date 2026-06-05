using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanoLogger.Domain.Roles;

namespace PanoLogger.Infrastructure.Persistence.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    private static readonly Guid SuperAdminRoleId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid CompanyAdminRoleId = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid FacilityManagerRoleId = new("33333333-3333-3333-3333-333333333333");
    private static readonly Guid ViewerRoleId = new("44444444-4444-4444-4444-444444444444");

    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");
        builder.HasKey(rolePermission => new { rolePermission.RoleId, rolePermission.Permission });

        builder.Property(rolePermission => rolePermission.Permission).HasMaxLength(120).IsRequired();
        builder.Property(rolePermission => rolePermission.GrantedAtUtc).IsRequired();

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(rolePermission => rolePermission.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            SeedRolePermissions(SuperAdminRoleId, AppPermissions.ForRoles([AppRoles.SuperAdmin]))
                .Concat(SeedRolePermissions(CompanyAdminRoleId, AppPermissions.ForRoles([AppRoles.CompanyAdmin])))
                .Concat(SeedRolePermissions(FacilityManagerRoleId, AppPermissions.ForRoles([AppRoles.FacilityManager])))
                .Concat(SeedRolePermissions(ViewerRoleId, AppPermissions.ForRoles([AppRoles.Viewer]))));
    }

    private static IEnumerable<object> SeedRolePermissions(Guid roleId, IEnumerable<string> permissions)
    {
        return permissions.Select(permission => new
        {
            RoleId = roleId,
            Permission = permission,
            GrantedAtUtc = DateTimeOffset.UnixEpoch,
        });
    }
}
