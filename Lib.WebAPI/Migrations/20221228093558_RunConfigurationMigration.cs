using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <summary>
    /// RunConfigurationMigration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class RunConfigurationMigration : Migration
    {
        /// <summary>
        /// Ups the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RunConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FlowchartTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaHierarchyLevels = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RunRestrictions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunConfigurations_FlowchartTemplates_FlowchartTemplateId",
                        column: x => x.FlowchartTemplateId,
                        principalTable: "FlowchartTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RunConfigurations_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RunConfigurations_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RunConfigurations_CreatedByUserId",
                table: "RunConfigurations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RunConfigurations_FlowchartTemplateId",
                table: "RunConfigurations",
                column: "FlowchartTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_RunConfigurations_ModifiedByUserId",
                table: "RunConfigurations",
                column: "ModifiedByUserId");
        }

        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RunConfigurations");
        }
    }
}
