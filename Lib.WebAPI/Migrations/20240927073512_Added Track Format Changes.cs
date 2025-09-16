using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedTrackFormatChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TrackFormatChanges",
                table: "RunConfigurations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TrackFormatChanges",
                table: "FlowchartTemplatesVersionHistorties",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackFormatChanges",
                table: "RunConfigurations");

            migrationBuilder.DropColumn(
                name: "TrackFormatChanges",
                table: "FlowchartTemplatesVersionHistorties");
        }
    }
}
