using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PanoLogger.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(PanoLoggerDbContext))]
    [Migration("20260605123000_AddFileDeletePermission")]
    public partial class AddFileDeletePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO role_permissions (role_id, permission, granted_at_utc)
                VALUES ('11111111-1111-1111-1111-111111111111', 'files.delete', TIMESTAMPTZ '1970-01-01 00:00:00+00')
                ON CONFLICT (role_id, permission) DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM role_permissions
                WHERE role_id = '11111111-1111-1111-1111-111111111111'
                  AND permission = 'files.delete';
                """);
        }
    }
}
