using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <summary>
    /// AthenaTableNameChangesMigration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class AthenaTableNameChangesMigration : Migration
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
            migrationBuilder.UpdateData(
                table: "DataDictionaryTables",
                keyColumn: "Id",
                keyValue: new Guid("604762ba-d137-44f9-a996-e51352d83699"),
                column: "Name",
                value: "clientrollperiod");

            migrationBuilder.UpdateData(
                table: "DataDictionaryTables",
                keyColumn: "Id",
                keyValue: new Guid("620bdd28-2967-440b-b25f-a5695986824b"),
                column: "Name",
                value: "rollperiod");
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
            migrationBuilder.UpdateData(
                table: "DataDictionaryTables",
                keyColumn: "Id",
                keyValue: new Guid("604762ba-d137-44f9-a996-e51352d83699"),
                column: "Name",
                value: "client_roll_period");

            migrationBuilder.UpdateData(
                table: "DataDictionaryTables",
                keyColumn: "Id",
                keyValue: new Guid("620bdd28-2967-440b-b25f-a5695986824b"),
                column: "Name",
                value: "roll_period");
        }
    }
}