using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PanoLogger.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "company_code",
                table: "companies",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE companies
                SET company_code = 'CMP-' || upper(substr(replace(id::text, '-', ''), 1, 8))
                WHERE company_code = '';
                """);

            migrationBuilder.UpdateData(
                table: "companies",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                column: "company_code",
                value: "AVM-001");

            migrationBuilder.UpdateData(
                table: "companies",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                column: "company_code",
                value: "ISY-001");

            migrationBuilder.UpdateData(
                table: "companies",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"),
                column: "company_code",
                value: "END-001");

            migrationBuilder.CreateIndex(
                name: "IX_companies_company_code",
                table: "companies",
                column: "company_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_companies_company_code",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "company_code",
                table: "companies");
        }
    }
}
