using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <summary>
    /// ClientIdMigration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class ClientIdMigration : Migration
    {
        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "TotalsTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "ThemeTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "SummaryTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "MediaHierarchyTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "HeaderTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "GrandTotalTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "FooterTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "FlowchartTemplates",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "CalendarTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "CalendarOverlayTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Name", "Removed" },
                values: new object[] { new Guid("8cab384b-e8b2-448a-af69-0d13ec0549c7"), "Volkswagen", false });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Name", "Removed" },
                values: new object[] { new Guid("ae728d04-c101-11e8-bc8b-12cc0f0e8006"), "Pepsi", false });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Name", "Removed" },
                values: new object[] { new Guid("d08a19a9-bfd6-4a1e-a58c-6ee84dc96762"), "Diageo", false });

            migrationBuilder.InsertData(
                table: "FlowchartTemplates",
                columns: new[] { "Id", "ClientId", "CreatedByUserId", "CreatedDate", "FlowchartDefinition", "ModifiedByUserId", "ModifiedDate", "Name", "Removed", "Version" },
                values: new object[,]
                {
                    { new Guid("08c9d0a6-5253-41e4-810f-9181bb6c3496"), new Guid("ae728d04-c101-11e8-bc8b-12cc0f0e8006"), new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "2022 Frito Lay Flowpack", false, 1 },
                    { new Guid("48c0fc57-6b91-492c-8e6a-82cffe665bbe"), new Guid("ae728d04-c101-11e8-bc8b-12cc0f0e8006"), new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "2022 Cheetos Print ATB", false, 1 },
                    { new Guid("6781239f-aafd-4f2e-865f-0b781179d1c2"), new Guid("ae728d04-c101-11e8-bc8b-12cc0f0e8006"), new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "2022 Period Spend FLNA", false, 1 },
                    { new Guid("cd331eb4-142d-4ba4-a7c6-4649e26bf16d"), new Guid("ae728d04-c101-11e8-bc8b-12cc0f0e8006"), new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "2022 Pure Leaf IWD ATB", false, 1 },
                });

            migrationBuilder.CreateIndex(
                name: "IX_TotalsTemplates_ClientId",
                table: "TotalsTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ThemeTemplates_ClientId",
                table: "ThemeTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryTemplates_ClientId",
                table: "SummaryTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaHierarchyTemplates_ClientId",
                table: "MediaHierarchyTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_HeaderTemplates_ClientId",
                table: "HeaderTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_GrandTotalTemplates_ClientId",
                table: "GrandTotalTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FooterTemplates_ClientId",
                table: "FooterTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowchartTemplates_ClientId",
                table: "FlowchartTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTemplates_ClientId",
                table: "CalendarTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarOverlayTemplates_ClientId",
                table: "CalendarOverlayTemplates",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarOverlayTemplates_Clients_ClientId",
                table: "CalendarOverlayTemplates",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarTemplates_Clients_ClientId",
                table: "CalendarTemplates",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowchartTemplates_Clients_ClientId",
                table: "FlowchartTemplates",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FooterTemplates_Clients_ClientId",
                table: "FooterTemplates",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GrandTotalTemplates_Clients_ClientId",
                table: "GrandTotalTemplates",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HeaderTemplates_Clients_ClientId",
                table: "HeaderTemplates",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaHierarchyTemplates_Clients_ClientId",
                table: "MediaHierarchyTemplates",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SummaryTemplates_Clients_ClientId",
                table: "SummaryTemplates",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThemeTemplates_Clients_ClientId",
                table: "ThemeTemplates",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TotalsTemplates_Clients_ClientId",
                table: "TotalsTemplates",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <summary>
        /// Ups the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM RunConfigurations");
            migrationBuilder.Sql("DELETE FROM Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarOverlayTemplates_Clients_ClientId",
                table: "CalendarOverlayTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarTemplates_Clients_ClientId",
                table: "CalendarTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowchartTemplates_Clients_ClientId",
                table: "FlowchartTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_FooterTemplates_Clients_ClientId",
                table: "FooterTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_GrandTotalTemplates_Clients_ClientId",
                table: "GrandTotalTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_HeaderTemplates_Clients_ClientId",
                table: "HeaderTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaHierarchyTemplates_Clients_ClientId",
                table: "MediaHierarchyTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_SummaryTemplates_Clients_ClientId",
                table: "SummaryTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_ThemeTemplates_Clients_ClientId",
                table: "ThemeTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_TotalsTemplates_Clients_ClientId",
                table: "TotalsTemplates");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_TotalsTemplates_ClientId",
                table: "TotalsTemplates");

            migrationBuilder.DropIndex(
                name: "IX_ThemeTemplates_ClientId",
                table: "ThemeTemplates");

            migrationBuilder.DropIndex(
                name: "IX_SummaryTemplates_ClientId",
                table: "SummaryTemplates");

            migrationBuilder.DropIndex(
                name: "IX_MediaHierarchyTemplates_ClientId",
                table: "MediaHierarchyTemplates");

            migrationBuilder.DropIndex(
                name: "IX_HeaderTemplates_ClientId",
                table: "HeaderTemplates");

            migrationBuilder.DropIndex(
                name: "IX_GrandTotalTemplates_ClientId",
                table: "GrandTotalTemplates");

            migrationBuilder.DropIndex(
                name: "IX_FooterTemplates_ClientId",
                table: "FooterTemplates");

            migrationBuilder.DropIndex(
                name: "IX_FlowchartTemplates_ClientId",
                table: "FlowchartTemplates");

            migrationBuilder.DropIndex(
                name: "IX_CalendarTemplates_ClientId",
                table: "CalendarTemplates");

            migrationBuilder.DropIndex(
                name: "IX_CalendarOverlayTemplates_ClientId",
                table: "CalendarOverlayTemplates");

            migrationBuilder.DeleteData(
                table: "FlowchartTemplates",
                keyColumn: "Id",
                keyValue: new Guid("08c9d0a6-5253-41e4-810f-9181bb6c3496"));

            migrationBuilder.DeleteData(
                table: "FlowchartTemplates",
                keyColumn: "Id",
                keyValue: new Guid("48c0fc57-6b91-492c-8e6a-82cffe665bbe"));

            migrationBuilder.DeleteData(
                table: "FlowchartTemplates",
                keyColumn: "Id",
                keyValue: new Guid("6781239f-aafd-4f2e-865f-0b781179d1c2"));

            migrationBuilder.DeleteData(
                table: "FlowchartTemplates",
                keyColumn: "Id",
                keyValue: new Guid("cd331eb4-142d-4ba4-a7c6-4649e26bf16d"));

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "TotalsTemplates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "ThemeTemplates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "SummaryTemplates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "MediaHierarchyTemplates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "HeaderTemplates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "GrandTotalTemplates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "FooterTemplates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "FlowchartTemplates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "CalendarTemplates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "CalendarOverlayTemplates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}