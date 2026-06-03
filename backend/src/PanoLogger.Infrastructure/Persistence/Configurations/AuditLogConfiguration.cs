using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanoLogger.Domain.AuditLogs;
using PanoLogger.Domain.Users;

namespace PanoLogger.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(auditLog => auditLog.Id);

        builder.Property(auditLog => auditLog.Action).HasMaxLength(120).IsRequired();
        builder.Property(auditLog => auditLog.EntityName).HasMaxLength(120).IsRequired();
        builder.Property(auditLog => auditLog.OccurredAtUtc).IsRequired();
        builder.Property(auditLog => auditLog.Metadata).HasColumnType("jsonb").HasDefaultValue("{}").IsRequired();
        builder.Property(auditLog => auditLog.CreatedAtUtc).IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(auditLog => auditLog.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(auditLog => auditLog.OccurredAtUtc);
        builder.HasIndex(auditLog => new { auditLog.EntityName, auditLog.EntityId });
    }
}
