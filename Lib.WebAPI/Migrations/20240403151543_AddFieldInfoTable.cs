using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    public partial class AddFieldInfoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMetric = table.Column<bool>(type: "bit", nullable: false),
                    IsCurrency = table.Column<bool>(type: "bit", nullable: false),
                    IsCommon = table.Column<bool>(type: "bit", nullable: false),
                    ModuleType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldInfos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldInfos");
        }
    }
}
