using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PanoLogger.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UseUsernameForAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_email",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "username");

            migrationBuilder.Sql(
                """
                WITH normalized_users AS (
                    SELECT
                        id,
                        COALESCE(NULLIF(lower(regexp_replace(split_part(username, '@', 1), '[^a-z0-9._-]', '-', 'g')), ''), 'user') AS base_username
                    FROM users
                ),
                ranked_users AS (
                    SELECT
                        id,
                        base_username,
                        row_number() OVER (PARTITION BY base_username ORDER BY id) AS duplicate_number
                    FROM normalized_users
                )
                UPDATE users
                SET username = CASE
                    WHEN ranked_users.duplicate_number = 1 THEN left(ranked_users.base_username, 80)
                    ELSE left(ranked_users.base_username, 68) || '-' || left(users.id::text, 8)
                END
                FROM ranked_users
                WHERE users.id = ranked_users.id;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "users",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(254)",
                oldMaxLength: 254);

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_username",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "users",
                type: "character varying(254)",
                maxLength: 254,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.RenameColumn(
                name: "username",
                table: "users",
                newName: "email");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }
    }
}
