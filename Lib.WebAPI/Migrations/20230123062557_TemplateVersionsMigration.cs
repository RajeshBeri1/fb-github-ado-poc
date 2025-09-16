using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <summary>
    /// TemplateVersionsMigration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class TemplateVersionsMigration : Migration
    {
        /// <summary>
        /// Ups the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "TotalsTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "ThemeTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "SummaryTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "MediaHierarchyTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "HeaderTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "GrandTotalTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "FooterTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "CalendarTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "CalendarOverlayTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "TotalsTemplates");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "ThemeTemplates");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "SummaryTemplates");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "MediaHierarchyTemplates");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "HeaderTemplates");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "GrandTotalTemplates");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "FooterTemplates");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "CalendarTemplates");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "CalendarOverlayTemplates");
        }
    }
}
