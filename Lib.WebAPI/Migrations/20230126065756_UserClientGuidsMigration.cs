using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <summary>
    /// UserClientGuidsMigration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class UserClientGuidsMigration : Migration
    {
        /// <summary>
        /// Ups the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM FlowchartTemplates WHERE CreatedByUserId = 'e82689ff-6a88-44a8-b393-21a92e7db593'");
            migrationBuilder.Sql("DELETE FROM CalendarOverlayTemplates WHERE CreatedByUserId = 'e82689ff-6a88-44a8-b393-21a92e7db593'");
            migrationBuilder.Sql("DELETE FROM CalendarTemplates WHERE CreatedByUserId = 'e82689ff-6a88-44a8-b393-21a92e7db593'");
            migrationBuilder.Sql("DELETE FROM FooterTemplates WHERE CreatedByUserId = 'e82689ff-6a88-44a8-b393-21a92e7db593'");
            migrationBuilder.Sql("DELETE FROM GrandTotalTemplates WHERE CreatedByUserId = 'e82689ff-6a88-44a8-b393-21a92e7db593'");
            migrationBuilder.Sql("DELETE FROM HeaderTemplates WHERE CreatedByUserId = 'e82689ff-6a88-44a8-b393-21a92e7db593'");
            migrationBuilder.Sql("DELETE FROM MediaHierarchyTemplates WHERE CreatedByUserId = 'e82689ff-6a88-44a8-b393-21a92e7db593'");
            migrationBuilder.Sql("DELETE FROM SummaryTemplates WHERE CreatedByUserId = 'e82689ff-6a88-44a8-b393-21a92e7db593'");
            migrationBuilder.Sql("DELETE FROM ThemeTemplates WHERE CreatedByUserId = 'e82689ff-6a88-44a8-b393-21a92e7db593'");
            migrationBuilder.Sql("DELETE FROM TotalsTemplates WHERE CreatedByUserId = 'e82689ff-6a88-44a8-b393-21a92e7db593'");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"));

            migrationBuilder.AddColumn<string>(
                name: "AllowedClientGuids",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty);
        }

        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedClientGuids",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DisplayName", "Name", "Removed" },
                values: new object[] { new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), "Bob Jones", "bob.jones@omnicommediagroup.com", false });
        }
    }
}
