using Microsoft.EntityFrameworkCore;
using PanoLogger.Domain.AuditLogs;
using PanoLogger.Domain.Companies;
using PanoLogger.Domain.Facilities;
using PanoLogger.Domain.Files;
using PanoLogger.Domain.MaintenanceReports;
using PanoLogger.Domain.Panels;
using PanoLogger.Domain.Roles;
using PanoLogger.Domain.Users;

namespace PanoLogger.Infrastructure.Persistence;

public sealed class PanoLoggerDbContext(DbContextOptions<PanoLoggerDbContext> options) : DbContext(options)
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<Panel> Panels => Set<Panel>();
    public DbSet<PanelFile> PanelFiles => Set<PanelFile>();
    public DbSet<PanelFileCategory> PanelFileCategories => Set<PanelFileCategory>();
    public DbSet<MaintenanceReport> MaintenanceReports => Set<MaintenanceReport>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PanoLoggerDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }
        }
    }

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var builder = new System.Text.StringBuilder(value.Length + 8);

        for (var index = 0; index < value.Length; index++)
        {
            var character = value[index];

            if (char.IsUpper(character) && index > 0)
            {
                builder.Append('_');
            }

            builder.Append(char.ToLowerInvariant(character));
        }

        return builder.ToString();
    }
}
