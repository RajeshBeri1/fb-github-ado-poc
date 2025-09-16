using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <summary>
    /// DataDictionaryTablesSeedMigration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class DataDictionaryTablesSeedMigration : Migration
    {
        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DataDictionaryTables",
                keyColumn: "Id",
                keyValue: new Guid("3e185d41-9e21-44dc-9532-cd4bda1b5a0d"));

            migrationBuilder.DeleteData(
                table: "DataDictionaryTables",
                keyColumn: "Id",
                keyValue: new Guid("6f2ce863-ece8-442c-b5fb-bd59075914a9"));

            migrationBuilder.DeleteData(
                table: "DataDictionaryTables",
                keyColumn: "Id",
                keyValue: new Guid("f0867476-5d91-4fbf-8ebf-c5308309f4ea"));
        }

        /// <summary>
        /// Ups the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM DataDictionaryTables");

            migrationBuilder.InsertData(
                table: "DataDictionaryTables",
                columns: new[] { "Id", "DataDictionaryColumns", "Name", "Removed" },
                values: new object[] { new Guid("3e185d41-9e21-44dc-9532-cd4bda1b5a0d"), string.Empty, "media_briefs", false });

            migrationBuilder.InsertData(
                table: "DataDictionaryTables",
                columns: new[] { "Id", "DataDictionaryColumns", "Name", "Removed" },
                values: new object[] { new Guid("6f2ce863-ece8-442c-b5fb-bd59075914a9"), string.Empty, "campaign", false });

            migrationBuilder.InsertData(
                table: "DataDictionaryTables",
                columns: new[] { "Id", "DataDictionaryColumns", "Name", "Removed" },
                values: new object[] { new Guid("f0867476-5d91-4fbf-8ebf-c5308309f4ea"), string.Empty, "media_plans", false });
        }
    }
}