using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanoLogger.Domain.Roles;

namespace PanoLogger.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(role => role.Id);

        builder.Property(role => role.Name).HasMaxLength(80).IsRequired();
        builder.Property(role => role.Description).HasMaxLength(300).IsRequired();
        builder.Property(role => role.CreatedAtUtc).IsRequired();

        builder.HasIndex(role => role.Name).IsUnique();

        builder.HasData(
            new
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                Name = AppRoles.SuperAdmin,
                Description = "Full system access across all companies, facilities, panels, files, QR codes, and users.",
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            },
            new
            {
                Id = new Guid("22222222-2222-2222-2222-222222222222"),
                Name = AppRoles.CompanyAdmin,
                Description = "Company-level administration for facilities, panels, files, QR codes, and reports.",
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            },
            new
            {
                Id = new Guid("33333333-3333-3333-3333-333333333333"),
                Name = AppRoles.FacilityManager,
                Description = "Facility-level management for panels, files, QR codes, and reports.",
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            },
            new
            {
                Id = new Guid("44444444-4444-4444-4444-444444444444"),
                Name = AppRoles.Viewer,
                Description = "Read-only access to reports and public panel information.",
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            });
    }
}
