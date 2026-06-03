using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanoLogger.Domain.MaintenanceReports;
using PanoLogger.Domain.Panels;
using PanoLogger.Domain.Users;

namespace PanoLogger.Infrastructure.Persistence.Configurations;

public sealed class MaintenanceReportConfiguration : IEntityTypeConfiguration<MaintenanceReport>
{
    public void Configure(EntityTypeBuilder<MaintenanceReport> builder)
    {
        builder.ToTable("maintenance_reports");
        builder.HasKey(report => report.Id);

        builder.Property(report => report.Title).HasMaxLength(180).IsRequired();
        builder.Property(report => report.ReportDateUtc).IsRequired();
        builder.Property(report => report.Notes).HasMaxLength(4000).IsRequired();
        builder.Property(report => report.CreatedAtUtc).IsRequired();

        builder.HasOne<Panel>()
            .WithMany()
            .HasForeignKey(report => report.PanelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(report => report.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(report => new { report.PanelId, report.ReportDateUtc });
    }
}
