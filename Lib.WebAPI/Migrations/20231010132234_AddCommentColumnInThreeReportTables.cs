using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    public partial class AddCommentColumnInThreeReportTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "RunConfigurations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Reports",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "FlowchartTemplatesVersionHistorties",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "RunConfigurations");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "FlowchartTemplatesVersionHistorties");
        }
    }
}
