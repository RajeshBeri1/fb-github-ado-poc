using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <summary>
    /// ReportMigration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class ReportMigration : Migration
    {
        /// <summary>
        /// Ups the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlowchartData",
                table: "FlowchartTemplates");

            migrationBuilder.DropColumn(
                name: "StorageFileId",
                table: "FlowchartTemplates");

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    FlowchartTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_FlowchartTemplates_FlowchartTemplateId",
                        column: x => x.FlowchartTemplateId,
                        principalTable: "FlowchartTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CreatedByUserId",
                table: "Reports",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_FlowchartTemplateId",
                table: "Reports",
                column: "FlowchartTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ModifiedByUserId",
                table: "Reports",
                column: "ModifiedByUserId");
        }

        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.AddColumn<string>(
                name: "FlowchartData",
                table: "FlowchartTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StorageFileId",
                table: "FlowchartTemplates",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
