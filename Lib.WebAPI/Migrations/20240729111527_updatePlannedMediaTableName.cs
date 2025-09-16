using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    public partial class updatePlannedMediaTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlannedMediaToTestSchemaMappings");

            migrationBuilder.CreateTable(
                name: "PlannedMediaMigrationColumnMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceColumnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationColumnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedMediaMigrationColumnMappings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlannedMediaMigrationColumnMappings");

            migrationBuilder.CreateTable(
                name: "PlannedMediaToTestSchemaMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationColumnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    SourceColumnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedMediaToTestSchemaMappings", x => x.Id);
                });
        }
    }
}
