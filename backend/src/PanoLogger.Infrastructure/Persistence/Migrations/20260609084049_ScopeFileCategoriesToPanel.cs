using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PanoLogger.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ScopeFileCategoriesToPanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""CREATE EXTENSION IF NOT EXISTS pgcrypto;""");

            migrationBuilder.DropIndex(
                name: "IX_panel_file_categories_key",
                table: "panel_file_categories");

            migrationBuilder.DropIndex(
                name: "IX_panel_file_categories_sort_order",
                table: "panel_file_categories");

            migrationBuilder.AddColumn<Guid>(
                name: "panel_id",
                table: "panel_file_categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                DELETE FROM panel_file_categories;

                INSERT INTO panel_file_categories (
                    id,
                    panel_id,
                    key,
                    name,
                    description,
                    icon,
                    sort_order,
                    is_system,
                    is_active,
                    created_at_utc,
                    updated_at_utc
                )
                SELECT
                    gen_random_uuid(),
                    p.id,
                    default_categories.key,
                    default_categories.name,
                    default_categories.description,
                    default_categories.icon,
                    default_categories.sort_order,
                    true,
                    true,
                    TIMESTAMPTZ '1970-01-01 00:00:00+00',
                    NULL
                FROM panels p
                CROSS JOIN (
                    VALUES
                        ('MaintenanceReport', 'Bakım', 'Periyodik bakım kayıtları', 'pi pi-wrench', 10),
                        ('ElectricalProject', 'Tek Hat', 'Tek hat ve elektrik proje dosyaları', 'pi pi-sitemap', 20),
                        ('PanelDocument', 'Proje', 'Yüklenen teknik proje dosyaları', 'pi pi-file', 30)
                ) AS default_categories(key, name, description, icon, sort_order);

                WITH used_custom_categories AS (
                    SELECT DISTINCT panel_id, category
                    FROM panel_files
                    WHERE category NOT IN ('MaintenanceReport', 'ElectricalProject', 'PanelDocument')
                ),
                ordered_custom_categories AS (
                    SELECT
                        panel_id,
                        category,
                        30 + (ROW_NUMBER() OVER (PARTITION BY panel_id ORDER BY category) * 10) AS sort_order
                    FROM used_custom_categories
                )
                INSERT INTO panel_file_categories (
                    id,
                    panel_id,
                    key,
                    name,
                    description,
                    icon,
                    sort_order,
                    is_system,
                    is_active,
                    created_at_utc,
                    updated_at_utc
                )
                SELECT
                    gen_random_uuid(),
                    panel_id,
                    category,
                    category,
                    category || ' dosyaları',
                    'pi pi-folder',
                    sort_order,
                    false,
                    true,
                    NOW(),
                    NULL
                FROM ordered_custom_categories;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "panel_id",
                table: "panel_file_categories",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_panel_file_categories_panel_id_key",
                table: "panel_file_categories",
                columns: new[] { "panel_id", "key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_panel_file_categories_panel_id_sort_order",
                table: "panel_file_categories",
                columns: new[] { "panel_id", "sort_order" });

            migrationBuilder.AddForeignKey(
                name: "FK_panel_file_categories_panels_panel_id",
                table: "panel_file_categories",
                column: "panel_id",
                principalTable: "panels",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_panel_file_categories_panels_panel_id",
                table: "panel_file_categories");

            migrationBuilder.DropIndex(
                name: "IX_panel_file_categories_panel_id_key",
                table: "panel_file_categories");

            migrationBuilder.DropIndex(
                name: "IX_panel_file_categories_panel_id_sort_order",
                table: "panel_file_categories");

            migrationBuilder.DropColumn(
                name: "panel_id",
                table: "panel_file_categories");

            migrationBuilder.Sql(
                """
                DELETE FROM panel_file_categories;

                INSERT INTO panel_file_categories (
                    id,
                    key,
                    name,
                    description,
                    icon,
                    sort_order,
                    is_system,
                    is_active,
                    created_at_utc,
                    updated_at_utc
                )
                VALUES
                    ('aaaa1111-1111-1111-1111-111111111111', 'MaintenanceReport', 'Bakım', 'Periyodik bakım kayıtları', 'pi pi-wrench', 10, true, true, TIMESTAMPTZ '1970-01-01 00:00:00+00', NULL),
                    ('aaaa2222-2222-2222-2222-222222222222', 'ElectricalProject', 'Tek Hat', 'Tek hat ve elektrik proje dosyaları', 'pi pi-sitemap', 20, true, true, TIMESTAMPTZ '1970-01-01 00:00:00+00', NULL),
                    ('aaaa3333-3333-3333-3333-333333333333', 'PanelDocument', 'Proje', 'Yüklenen teknik proje dosyaları', 'pi pi-file', 30, true, true, TIMESTAMPTZ '1970-01-01 00:00:00+00', NULL);
                """);

            migrationBuilder.CreateIndex(
                name: "IX_panel_file_categories_key",
                table: "panel_file_categories",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_panel_file_categories_sort_order",
                table: "panel_file_categories",
                column: "sort_order");
        }
    }
}
