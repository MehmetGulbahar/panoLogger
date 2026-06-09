using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanoLogger.Domain.Files;
using PanoLogger.Domain.Panels;

namespace PanoLogger.Infrastructure.Persistence.Configurations;

public sealed class PanelFileConfiguration : IEntityTypeConfiguration<PanelFile>
{
    public void Configure(EntityTypeBuilder<PanelFile> builder)
    {
        builder.ToTable("panel_files");
        builder.HasKey(file => file.Id);

        builder.Property(file => file.Category).HasMaxLength(80).IsRequired();
        builder.Property(file => file.FileName).HasMaxLength(255).IsRequired();
        builder.Property(file => file.StoragePath).HasMaxLength(1024).IsRequired();
        builder.Property(file => file.ContentType).HasMaxLength(120).IsRequired();
        builder.Property(file => file.SizeBytes).IsRequired();
        builder.Property(file => file.CreatedAtUtc).IsRequired();

        builder.HasOne<Panel>()
            .WithMany()
            .HasForeignKey(file => file.PanelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(file => new { file.PanelId, file.Category });
        builder.HasIndex(file => file.StoragePath).IsUnique();
    }
}
