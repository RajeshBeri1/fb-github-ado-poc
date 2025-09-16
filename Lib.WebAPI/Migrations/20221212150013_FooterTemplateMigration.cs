using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <summary>
    /// FooterTemplateMigration
    /// </summary>
    public partial class FooterTemplateMigration : Migration
    {
        /// <summary>
        /// <para>Builds the operations that will migrate the database 'down'.</para>
        /// <para>
        /// That is, builds the operations that will take the database from the state left
        /// in by this migration so that it returns to the state that it was in before
        /// this migration was applied.
        /// </para>
        /// <para>
        /// This method must be overridden in each class that inherits from <see
        /// cref="T:Microsoft.EntityFrameworkCore.Migrations.Migration" /> if both 'up'
        /// and 'down' migrations are to be supported. If it is not overridden, then
        /// calling it will throw and it will not be possible to migrate in the 'down'
        /// direction.
        /// </para>
        /// </summary>
        /// <param name="migrationBuilder">
        /// The <see cref="T:Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder" />
        /// that will build the operations.
        /// </param>
        /// <remarks>
        /// See <see href="https://aka.ms/efcore-docs-migrations">Database
        /// migrations</see> for more information.
        /// </remarks>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FooterTemplates");
        }

        /// <summary>
        /// <para>Builds the operations that will migrate the database 'up'.</para>
        /// <para>
        /// That is, builds the operations that will take the database from the state left
        /// in by the previous migration so that it is up-to-date with regard to this
        /// migration.
        /// </para>
        /// <para>
        /// This method must be overridden in each class that inherits from <see
        /// cref="T:Microsoft.EntityFrameworkCore.Migrations.Migration" />.
        /// </para>
        /// </summary>
        /// <param name="migrationBuilder">
        /// The <see cref="T:Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder" />
        /// that will build the operations.
        /// </param>
        /// <remarks>
        /// See <see href="https://aka.ms/efcore-docs-migrations">Database
        /// migrations</see> for more information.
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FooterTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FooterDefinition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FooterTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FooterTemplates_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FooterTemplates_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FooterTemplates_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FooterTemplates_ClientId",
                table: "FooterTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FooterTemplates_CreatedByUserId",
                table: "FooterTemplates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FooterTemplates_ModifiedByUserId",
                table: "FooterTemplates",
                column: "ModifiedByUserId");
        }
    }
}