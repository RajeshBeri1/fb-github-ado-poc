using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    public partial class AddClientMappingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientMapping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientMapping_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "DataDictionaryTables",
                columns: new[] { "Id", "DataDictionaryColumns", "Name", "Removed" },
                values: new object[,]
                {
                    { new Guid("0d771867-cc9a-40a5-900e-f57528f91d0e"), "", "supplier", false },
                    { new Guid("a1b4a058-d33d-4e9b-950c-5ca3cd438a7c"), "", "placement", false },
                    { new Guid("e13dda1a-48a3-4e26-8a76-620bb10a2c6c"), "", "channel", false },
                    { new Guid("e5cdf0ab-0455-43f7-953e-6b230513c3e6"), "", "budget", false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientMapping_ClientId",
                table: "ClientMapping",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientMapping");

            migrationBuilder.DeleteData(
                table: "DataDictionaryTables",
                keyColumn: "Id",
                keyValue: new Guid("0d771867-cc9a-40a5-900e-f57528f91d0e"));

            migrationBuilder.DeleteData(
                table: "DataDictionaryTables",
                keyColumn: "Id",
                keyValue: new Guid("a1b4a058-d33d-4e9b-950c-5ca3cd438a7c"));

            migrationBuilder.DeleteData(
                table: "DataDictionaryTables",
                keyColumn: "Id",
                keyValue: new Guid("e13dda1a-48a3-4e26-8a76-620bb10a2c6c"));

            migrationBuilder.DeleteData(
                table: "DataDictionaryTables",
                keyColumn: "Id",
                keyValue: new Guid("e5cdf0ab-0455-43f7-953e-6b230513c3e6"));
        }
    }
}
