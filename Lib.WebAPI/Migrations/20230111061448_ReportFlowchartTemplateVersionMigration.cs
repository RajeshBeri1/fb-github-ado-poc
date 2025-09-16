using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <summary>
    /// ReportFlowchartTemplateVersionMigration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class ReportFlowchartTemplateVersionMigration : Migration
    {
        /// <summary>
        /// Ups the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FlowchartTemplateVersion",
                table: "Reports",
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
                name: "FlowchartTemplateVersion",
                table: "Reports");
        }
    }
}
