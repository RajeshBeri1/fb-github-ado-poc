using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDefaultField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "TotalsTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "ThemeTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "MediaHierarchyTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "HeaderTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "GrandTotalTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "FooterTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "FlowchartTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "CalendarTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "CalendarOverlayTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "TotalsTemplates");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "ThemeTemplates");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "MediaHierarchyTemplates");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "HeaderTemplates");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "GrandTotalTemplates");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "FooterTemplates");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "FlowchartTemplates");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "CalendarTemplates");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "CalendarOverlayTemplates");
        }
    }
}
