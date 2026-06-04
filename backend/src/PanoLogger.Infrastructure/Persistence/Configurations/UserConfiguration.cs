using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanoLogger.Domain.Companies;
using PanoLogger.Domain.Users;

namespace PanoLogger.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Email).HasMaxLength(254).IsRequired();
        builder.Property(user => user.CompanyId);
        builder.Property(user => user.DisplayName).HasMaxLength(160).IsRequired();
        builder.Property(user => user.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(user => user.IsActive).IsRequired();
        builder.Property(user => user.CreatedAtUtc).IsRequired();

        builder.HasIndex(user => user.Email).IsUnique();
        builder.HasIndex(user => user.CompanyId);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(user => user.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
