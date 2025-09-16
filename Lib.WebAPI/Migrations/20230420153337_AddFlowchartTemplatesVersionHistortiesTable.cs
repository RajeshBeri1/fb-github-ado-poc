using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    public partial class AddFlowchartTemplatesVersionHistortiesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlowchartTemplatesVersionHistorties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowchartTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    FlowchartDefinition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowchartTemplatesVersionHistorties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowchartTemplatesVersionHistorties_FlowchartTemplates_FlowchartTemplateId",
                        column: x => x.FlowchartTemplateId,
                        principalTable: "FlowchartTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlowchartTemplatesVersionHistorties_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlowchartTemplatesVersionHistorties_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlowchartTemplatesVersionHistorties_CreatedByUserId",
                table: "FlowchartTemplatesVersionHistorties",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowchartTemplatesVersionHistorties_FlowchartTemplateId",
                table: "FlowchartTemplatesVersionHistorties",
                column: "FlowchartTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowchartTemplatesVersionHistorties_ModifiedByUserId",
                table: "FlowchartTemplatesVersionHistorties",
                column: "ModifiedByUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlowchartTemplatesVersionHistorties");
        }
    }
}
