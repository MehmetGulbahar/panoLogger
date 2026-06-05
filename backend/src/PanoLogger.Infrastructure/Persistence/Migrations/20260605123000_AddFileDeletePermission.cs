using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PanoLogger.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    [Migration("20260605123000_AddFileDeletePermission")]
    public partial class AddFileDeletePermission : Migration
    {
        private static readonly Guid SuperAdminRoleId = new("11111111-1111-1111-1111-111111111111");

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission", "role_id", "granted_at_utc" },
                values: new object[]
                {
                    "files.delete",
                    SuperAdminRoleId,
                    new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), TimeSpan.Zero)
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "role_id", "permission" },
                keyValues: new object[] { SuperAdminRoleId, "files.delete" });
        }
    }
}
