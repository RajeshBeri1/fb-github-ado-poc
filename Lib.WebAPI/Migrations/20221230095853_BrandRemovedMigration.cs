using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <summary>
    /// BrandRemovedMigration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class BrandRemovedMigration : Migration
    {
        /// <summary>
        /// Ups the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "FlowchartTemplates");
        }

        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "FlowchartTemplates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.UpdateData(
                table: "FlowchartTemplates",
                keyColumn: "Id",
                keyValue: new Guid("08c9d0a6-5253-41e4-810f-9181bb6c3496"),
                column: "Brand",
                value: "Pepsi MAX");

            migrationBuilder.UpdateData(
                table: "FlowchartTemplates",
                keyColumn: "Id",
                keyValue: new Guid("48c0fc57-6b91-492c-8e6a-82cffe665bbe"),
                column: "Brand",
                value: "Pepsi MAX");

            migrationBuilder.UpdateData(
                table: "FlowchartTemplates",
                keyColumn: "Id",
                keyValue: new Guid("6781239f-aafd-4f2e-865f-0b781179d1c2"),
                column: "Brand",
                value: "Pepsi MAX");

            migrationBuilder.UpdateData(
                table: "FlowchartTemplates",
                keyColumn: "Id",
                keyValue: new Guid("cd331eb4-142d-4ba4-a7c6-4649e26bf16d"),
                column: "Brand",
                value: "Pepsi MAX");
        }
    }
}
