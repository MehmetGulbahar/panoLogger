using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanoLogger.Domain.Companies;

namespace PanoLogger.Infrastructure.Persistence.Configurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");
        builder.HasKey(company => company.Id);

        builder.Property(company => company.Name).HasMaxLength(160).IsRequired();
        builder.Property(company => company.ProjectName).HasMaxLength(160).IsRequired();
        builder.Property(company => company.TaxNumber).HasMaxLength(40).IsRequired();
        builder.Property(company => company.Address).HasMaxLength(500).IsRequired();
        builder.Property(company => company.ContactEmail).HasMaxLength(254).IsRequired();
        builder.Property(company => company.CreatedAtUtc).IsRequired();

        builder.HasIndex(company => company.TaxNumber).IsUnique();
        builder.HasIndex(company => company.ContactEmail);

        builder.HasData(
            new
            {
                Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                Name = "Enerji Teknolojileri A.S.",
                ProjectName = "AVM Elektrik Yonetimi",
                TaxNumber = "1234567890",
                Address = "Ataturk Mah. Caliskan Cd. No:12, Istanbul",
                ContactEmail = "info@enerjitek.com",
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            },
            new
            {
                Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                Name = "Panel Yapi Ltd.",
                ProjectName = "Isyeri Elektrik Yonetimi",
                TaxNumber = "9876543210",
                Address = "Gazi Bulv. No:45, Izmir",
                ContactEmail = "iletisim@panelyapi.com",
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            },
            new
            {
                Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"),
                Name = "Elektrik Sistemleri Tic. Ltd.",
                ProjectName = "Endustriyel Elektrik Yonetimi",
                TaxNumber = "5647382910",
                Address = "Cumhuriyet Mh. Sanayi Sk. No:7, Ankara",
                ContactEmail = "destek@elektriksistem.com",
                CreatedAtUtc = DateTimeOffset.UnixEpoch,
                UpdatedAtUtc = (DateTimeOffset?)null,
            });
    }
}
