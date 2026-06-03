using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanoLogger.Domain.Companies;
using PanoLogger.Domain.Facilities;

namespace PanoLogger.Infrastructure.Persistence.Configurations;

public sealed class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.ToTable("facilities");
        builder.HasKey(facility => facility.Id);

        builder.Property(facility => facility.Name).HasMaxLength(160).IsRequired();
        builder.Property(facility => facility.City).HasMaxLength(100).IsRequired();
        builder.Property(facility => facility.District).HasMaxLength(100).IsRequired();
        builder.Property(facility => facility.Address).HasMaxLength(500).IsRequired();
        builder.Property(facility => facility.CreatedAtUtc).IsRequired();

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(facility => facility.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(facility => new { facility.CompanyId, facility.Name }).IsUnique();

        builder.HasData(
            new
            {
                Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"),
                CompanyId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                Name = "Merkez Uretim Tesisleri",
                City = "Istanbul",
                District = "Tuzla",
                Address = "Organize Sanayi Bolgesi, Parsel 34",
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            },
            new
            {
                Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"),
                CompanyId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                Name = "Depo ve Lojistik",
                City = "Izmir",
                District = "Bornova",
                Address = "Kargo Sok. No:8",
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            },
            new
            {
                Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"),
                CompanyId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"),
                Name = "Ar-Ge Merkezi",
                City = "Ankara",
                District = "Cankaya",
                Address = "Teknokent Blok A, Kat 3",
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            });
    }
}
