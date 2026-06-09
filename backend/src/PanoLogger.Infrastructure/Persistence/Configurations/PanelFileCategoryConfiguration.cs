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

        builder.Property(category => category.PanelId).IsRequired();
        builder.Property(category => category.Key).HasMaxLength(80).IsRequired();
        builder.Property(category => category.Name).HasMaxLength(120).IsRequired();
        builder.Property(category => category.Description).HasMaxLength(240).IsRequired();
        builder.Property(category => category.Icon).HasMaxLength(80).IsRequired();
        builder.Property(category => category.SortOrder).IsRequired();
        builder.Property(category => category.IsSystem).IsRequired();
        builder.Property(category => category.IsActive).IsRequired();
        builder.Property(category => category.CreatedAtUtc).IsRequired();
        builder.Property(category => category.UpdatedAtUtc);

        builder.HasIndex(category => new { category.PanelId, category.Key }).IsUnique();
        builder.HasIndex(category => new { category.PanelId, category.SortOrder });

        builder.HasOne<PanoLogger.Domain.Panels.Panel>()
            .WithMany()
            .HasForeignKey(category => category.PanelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
