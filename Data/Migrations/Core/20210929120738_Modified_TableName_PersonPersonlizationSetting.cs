using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations.core
{
    public partial class Modified_TableName_PersonPersonlizationSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonPersonalization");

            migrationBuilder.CreateTable(
                name: "PersonPersonalizationSetting",
                columns: table => new
                {
                    PersonPersonalizationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    ThemeListItemId = table.Column<int>(type: "integer", nullable: false),
                    TimeZone = table.Column<string>(type: "text", nullable: true),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonPersonalizationSetting", x => x.PersonPersonalizationId);
                    table.ForeignKey(
                        name: "FK_PersonPersonalizationSetting_ListItem_ThemeListItemId",
                        column: x => x.ThemeListItemId,
                        principalTable: "ListItem",
                        principalColumn: "ListItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonPersonalizationSetting_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonPersonalizationSetting_PersonId",
                table: "PersonPersonalizationSetting",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonPersonalizationSetting_ThemeListItemId",
                table: "PersonPersonalizationSetting",
                column: "ThemeListItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonPersonalizationSetting");

            migrationBuilder.CreateTable(
                name: "PersonPersonalization",
                columns: table => new
                {
                    PersonPersonalizationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    ThemeListItemId = table.Column<int>(type: "integer", nullable: false),
                    TimeZone = table.Column<string>(type: "text", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonPersonalization", x => x.PersonPersonalizationId);
                    table.ForeignKey(
                        name: "FK_PersonPersonalization_ListItem_ThemeListItemId",
                        column: x => x.ThemeListItemId,
                        principalTable: "ListItem",
                        principalColumn: "ListItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonPersonalization_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonPersonalization_PersonId",
                table: "PersonPersonalization",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonPersonalization_ThemeListItemId",
                table: "PersonPersonalization",
                column: "ThemeListItemId");
        }
    }
}
