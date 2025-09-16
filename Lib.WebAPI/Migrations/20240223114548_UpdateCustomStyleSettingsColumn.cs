using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    public partial class UpdateCustomStyleSettingsColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RunConfigCustomStyleSettings",
                table: "RunConfigurations",
                newName: "CustomStyleSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomStyleSettings",
                table: "RunConfigurations",
                newName: "RunConfigCustomStyleSettings");
        }
    }
}
