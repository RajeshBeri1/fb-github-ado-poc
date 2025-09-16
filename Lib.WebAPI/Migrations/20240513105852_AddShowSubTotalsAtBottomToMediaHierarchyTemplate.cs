using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    public partial class AddShowSubTotalsAtBottomToMediaHierarchyTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowSubTotalsAtBottom",
                table: "MediaHierarchyTemplates",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowSubTotalsAtBottom",
                table: "MediaHierarchyTemplates");
        }
    }
}
