using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanoLogger.Domain.Files;

namespace PanoLogger.Infrastructure.Persistence.Configurations;

public sealed class PanelFileCategoryConfiguration : IEntityTypeConfiguration<PanelFileCategory>
{
    public void Configure(EntityTypeBuilder<PanelFileCategory> builder)
    {
        builder.ToTable("panel_file_categories");
        builder.HasKey(category => category.Id);

        builder.Property(category => category.Key).HasMaxLength(80).IsRequired();
        builder.Property(category => category.Name).HasMaxLength(120).IsRequired();
        builder.Property(category => category.Description).HasMaxLength(240).IsRequired();
        builder.Property(category => category.Icon).HasMaxLength(80).IsRequired();
        builder.Property(category => category.SortOrder).IsRequired();
        builder.Property(category => category.IsSystem).IsRequired();
        builder.Property(category => category.IsActive).IsRequired();
        builder.Property(category => category.CreatedAtUtc).IsRequired();
        builder.Property(category => category.UpdatedAtUtc);

        builder.HasIndex(category => category.Key).IsUnique();
        builder.HasIndex(category => category.SortOrder);

        builder.HasData(
            new
            {
                Id = new Guid("aaaa1111-1111-1111-1111-111111111111"),
                Key = "MaintenanceReport",
                Name = "Bakım",
                Description = "Periyodik bakım kayıtları",
                Icon = "pi pi-wrench",
                SortOrder = 10,
                IsSystem = true,
                IsActive = true,
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            },
            new
            {
                Id = new Guid("aaaa2222-2222-2222-2222-222222222222"),
                Key = "ElectricalProject",
                Name = "Tek Hat",
                Description = "Tek hat ve elektrik proje dosyaları",
                Icon = "pi pi-sitemap",
                SortOrder = 20,
                IsSystem = true,
                IsActive = true,
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            },
            new
            {
                Id = new Guid("aaaa3333-3333-3333-3333-333333333333"),
                Key = "PanelDocument",
                Name = "Proje",
                Description = "Yüklenen teknik proje dosyaları",
                Icon = "pi pi-file",
                SortOrder = 30,
                IsSystem = true,
                IsActive = true,
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            });
    }
}
