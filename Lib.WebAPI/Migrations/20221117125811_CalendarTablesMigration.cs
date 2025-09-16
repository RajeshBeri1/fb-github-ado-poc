using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <summary>
    /// CalendarTablesMigration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class CalendarTablesMigration : Migration
    {
        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientColumnAliases",
                table: "ClientColumnAliases");

            migrationBuilder.DeleteData(
                table: "DataDictionaryTables",
                keyColumn: "Id",
                keyValue: new Guid("604762ba-d137-44f9-a996-e51352d83699"));

            migrationBuilder.DeleteData(
                table: "DataDictionaryTables",
                keyColumn: "Id",
                keyValue: new Guid("620bdd28-2967-440b-b25f-a5695986824b"));

            migrationBuilder.RenameTable(
                name: "ClientColumnAliases",
                newName: "ClientColumnAlias");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientColumnAlias",
                table: "ClientColumnAlias",
                column: "Id");
        }

        /// <summary>
        /// Ups the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientColumnAlias",
                table: "ClientColumnAlias");

            migrationBuilder.RenameTable(
                name: "ClientColumnAlias",
                newName: "ClientColumnAliases");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientColumnAliases",
                table: "ClientColumnAliases",
                column: "Id");

            migrationBuilder.InsertData(
                table: "DataDictionaryTables",
                columns: new[] { "Id", "DataDictionaryColumns", "Name", "Removed" },
                values: new object[] { new Guid("604762ba-d137-44f9-a996-e51352d83699"), string.Empty, "clientrollperiod", false });

            migrationBuilder.InsertData(
                table: "DataDictionaryTables",
                columns: new[] { "Id", "DataDictionaryColumns", "Name", "Removed" },
                values: new object[] { new Guid("620bdd28-2967-440b-b25f-a5695986824b"), string.Empty, "rollperiod", false });
        }
    }
}