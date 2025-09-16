using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lib.WebAPI.Migrations
{
    /// <summary>
    /// InitialMigration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class InitialMigration : Migration
    {
        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BroadcastCalendars");

            migrationBuilder.DropTable(
                name: "CalendarTemplates");

            migrationBuilder.DropTable(
                name: "ClientCalendars");

            migrationBuilder.DropTable(
                name: "ClientColumnAlias");

            migrationBuilder.DropTable(
                name: "DataDictionaryTables");

            migrationBuilder.DropTable(
                name: "FlowchartTemplates");

            migrationBuilder.DropTable(
                name: "GrandTotalTemplates");

            migrationBuilder.DropTable(
                name: "HeaderTemplates");

            migrationBuilder.DropTable(
                name: "MediaHierarchyTemplates");

            migrationBuilder.DropTable(
                name: "ThemeTemplates");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Users");
        }

        /// <summary>
        /// Ups the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BroadcastCalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BroadcastCalendarItems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<long>(type: "bigint", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BroadcastCalendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientCalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientCalendarItems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCalendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientColumnAlias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColumnAliases = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientColumnAlias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataDictionaryTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataDictionaryColumns = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataDictionaryTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EMail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalendarDefinition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarTemplates_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarTemplates_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarTemplates_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FlowchartTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FlowchartData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlowchartDefinition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StorageFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowchartTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowchartTemplates_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlowchartTemplates_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlowchartTemplates_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GrandTotalTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrandTotalDefinition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrandTotalTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrandTotalTemplates_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GrandTotalTemplates_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GrandTotalTemplates_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HeaderTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeaderDefinition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeaderTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeaderTemplates_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HeaderTemplates_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HeaderTemplates_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MediaHierarchyTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MediaHierarchyDefinition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaHierarchyTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaHierarchyTemplates_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MediaHierarchyTemplates_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MediaHierarchyTemplates_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThemeTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThemeDefinition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThemeTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThemeTemplates_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThemeTemplates_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThemeTemplates_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Name", "Removed" },
                values: new object[,]
                {
                    { new Guid("8cab384b-e8b2-448a-af69-0d13ec0549c7"), "Volkswagen", false },
                    { new Guid("ae728d04-c101-11e8-bc8b-12cc0f0e8006"), "Pepsi", false },
                    { new Guid("d08a19a9-bfd6-4a1e-a58c-6ee84dc96762"), "Diageo", false },
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DisplayName", "EMail", "Removed" },
                values: new object[] { new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), "Bob Jones", "bob.jones@omnicommediagroup.com", false });

            migrationBuilder.InsertData(
                table: "FlowchartTemplates",
                columns: new[] { "Id", "Brand", "ClientId", "CreatedByUserId", "CreatedDate", "FlowchartData", "FlowchartDefinition", "ModifiedByUserId", "ModifiedDate", "Name", "Removed", "StorageFileId", "Version" },
                values: new object[,]
                {
                    { new Guid("08c9d0a6-5253-41e4-810f-9181bb6c3496"), "Pepsi MAX", new Guid("ae728d04-c101-11e8-bc8b-12cc0f0e8006"), new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "2022 Frito Lay Flowpack", false, null, 1 },
                    { new Guid("48c0fc57-6b91-492c-8e6a-82cffe665bbe"), "Pepsi MAX", new Guid("ae728d04-c101-11e8-bc8b-12cc0f0e8006"), new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "2022 Cheetos Print ATB", false, null, 1 },
                    { new Guid("6781239f-aafd-4f2e-865f-0b781179d1c2"), "Pepsi MAX", new Guid("ae728d04-c101-11e8-bc8b-12cc0f0e8006"), new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "2022 Period Spend FLNA", false, null, 1 },
                    { new Guid("cd331eb4-142d-4ba4-a7c6-4649e26bf16d"), "Pepsi MAX", new Guid("ae728d04-c101-11e8-bc8b-12cc0f0e8006"), new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, new Guid("e82689ff-6a88-44a8-b393-21a92e7db593"), new DateTime(2022, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "2022 Pure Leaf IWD ATB", false, null, 1 },
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTemplates_ClientId",
                table: "CalendarTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTemplates_CreatedByUserId",
                table: "CalendarTemplates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTemplates_ModifiedByUserId",
                table: "CalendarTemplates",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowchartTemplates_ClientId",
                table: "FlowchartTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowchartTemplates_CreatedByUserId",
                table: "FlowchartTemplates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowchartTemplates_ModifiedByUserId",
                table: "FlowchartTemplates",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GrandTotalTemplates_ClientId",
                table: "GrandTotalTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_GrandTotalTemplates_CreatedByUserId",
                table: "GrandTotalTemplates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GrandTotalTemplates_ModifiedByUserId",
                table: "GrandTotalTemplates",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeaderTemplates_ClientId",
                table: "HeaderTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_HeaderTemplates_CreatedByUserId",
                table: "HeaderTemplates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeaderTemplates_ModifiedByUserId",
                table: "HeaderTemplates",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaHierarchyTemplates_ClientId",
                table: "MediaHierarchyTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaHierarchyTemplates_CreatedByUserId",
                table: "MediaHierarchyTemplates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaHierarchyTemplates_ModifiedByUserId",
                table: "MediaHierarchyTemplates",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ThemeTemplates_ClientId",
                table: "ThemeTemplates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ThemeTemplates_CreatedByUserId",
                table: "ThemeTemplates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ThemeTemplates_ModifiedByUserId",
                table: "ThemeTemplates",
                column: "ModifiedByUserId");
        }
    }
}