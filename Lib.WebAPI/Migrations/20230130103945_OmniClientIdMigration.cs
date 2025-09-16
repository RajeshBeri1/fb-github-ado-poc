using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <summary>
    /// OmniClientIdMigration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class OmniClientIdMigration : Migration
    {
        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OmniClientId",
                table: "TotalsTemplates");

            migrationBuilder.DropColumn(
                name: "OmniClientId",
                table: "ThemeTemplates");

            migrationBuilder.DropColumn(
                name: "OmniClientId",
                table: "SummaryTemplates");

            migrationBuilder.DropColumn(
                name: "OmniClientId",
                table: "MediaHierarchyTemplates");

            migrationBuilder.DropColumn(
                name: "OmniClientId",
                table: "HeaderTemplates");

            migrationBuilder.DropColumn(
                name: "OmniClientId",
                table: "GrandTotalTemplates");

            migrationBuilder.DropColumn(
                name: "OmniClientId",
                table: "FooterTemplates");

            migrationBuilder.DropColumn(
                name: "OmniClientId",
                table: "FlowchartTemplates");

            migrationBuilder.DropColumn(
                name: "OmniClientId",
                table: "CalendarTemplates");

            migrationBuilder.DropColumn(
                name: "OmniClientId",
                table: "CalendarOverlayTemplates");

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "TotalsTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ThemeTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "SummaryTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "MediaHierarchyTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "HeaderTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "GrandTotalTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "FooterTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "FlowchartTemplates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "CalendarTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "CalendarOverlayTemplates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <summary>
        /// Ups the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM RunConfigurations");
            migrationBuilder.Sql("DELETE FROM Reports");
            migrationBuilder.Sql("DELETE FROM FlowchartTemplates");
            migrationBuilder.Sql("DELETE FROM CalendarOverlayTemplates");
            migrationBuilder.Sql("DELETE FROM CalendarTemplates");
            migrationBuilder.Sql("DELETE FROM FooterTemplates");
            migrationBuilder.Sql("DELETE FROM GrandTotalTemplates");
            migrationBuilder.Sql("DELETE FROM HeaderTemplates");
            migrationBuilder.Sql("DELETE FROM MediaHierarchyTemplates");
            migrationBuilder.Sql("DELETE FROM SummaryTemplates");
            migrationBuilder.Sql("DELETE FROM ThemeTemplates");
            migrationBuilder.Sql("DELETE FROM TotalsTemplates");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "TotalsTemplates");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ThemeTemplates");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "SummaryTemplates");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "MediaHierarchyTemplates");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "HeaderTemplates");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "GrandTotalTemplates");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "FooterTemplates");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "FlowchartTemplates");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "CalendarTemplates");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "CalendarOverlayTemplates");

            migrationBuilder.AddColumn<Guid>(
                name: "OmniClientId",
                table: "TotalsTemplates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OmniClientId",
                table: "ThemeTemplates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OmniClientId",
                table: "SummaryTemplates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OmniClientId",
                table: "MediaHierarchyTemplates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OmniClientId",
                table: "HeaderTemplates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OmniClientId",
                table: "GrandTotalTemplates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OmniClientId",
                table: "FooterTemplates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OmniClientId",
                table: "FlowchartTemplates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OmniClientId",
                table: "CalendarTemplates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OmniClientId",
                table: "CalendarOverlayTemplates",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}