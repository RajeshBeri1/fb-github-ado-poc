using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    public partial class AddCurrencyInMediaHierarchy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "MediaHierarchyTemplates",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true,
                defaultValue: "LLL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "MediaHierarchyTemplates");
        }
    }
}
