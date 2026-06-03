using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanoLogger.Domain.Facilities;
using PanoLogger.Domain.Panels;

namespace PanoLogger.Infrastructure.Persistence.Configurations;

public sealed class PanelConfiguration : IEntityTypeConfiguration<Panel>
{
    public void Configure(EntityTypeBuilder<Panel> builder)
    {
        builder.ToTable("panels");
        builder.HasKey(panel => panel.Id);

        builder.Property(panel => panel.Code).HasMaxLength(80).IsRequired();
        builder.Property(panel => panel.Name).HasMaxLength(160).IsRequired();
        builder.Property(panel => panel.Description).HasMaxLength(800).IsRequired();
        builder.Property(panel => panel.CreatedAtUtc).IsRequired();

        builder.HasOne<Facility>()
            .WithMany()
            .HasForeignKey(panel => panel.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(panel => panel.Code).IsUnique();
        builder.HasIndex(panel => new { panel.FacilityId, panel.Name }).IsUnique();

        builder.HasData(
            new
            {
                Id = new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc1"),
                FacilityId = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"),
                Code = "P-001",
                Name = "Ana Dagitim Paneli",
                Description = "Tesis ana dagitim panosu, 400V, 1600A",
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            },
            new
            {
                Id = new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc2"),
                FacilityId = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"),
                Code = "P-042",
                Name = "Uretim Hatti Paneli",
                Description = "Uretim hatti besleme panosu, 230V, 200A",
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            },
            new
            {
                Id = new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc3"),
                FacilityId = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"),
                Code = "P-100",
                Name = "Test Laboratuvari Paneli",
                Description = "Test laboratuvari panosu, izolasyon ve olcum noktalari",
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            });
    }
}
