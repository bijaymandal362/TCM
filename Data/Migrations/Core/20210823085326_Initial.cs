using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations.core
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.CreateTable(
                name: "ListItemCategory",
                columns: table => new
                {
                    ListItemCategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ListItemCategoryName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ListItemCategorySystemName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsSystemConfig = table.Column<bool>(type: "boolean", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListItemCategory", x => x.ListItemCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.PersonId);
                });

            migrationBuilder.CreateTable(
                name: "ListItem",
                columns: table => new
                {
                    ListItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ListItemCategoryId = table.Column<int>(type: "integer", nullable: false),
                    ListItemName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ListItemSystemName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsSystemConfig = table.Column<bool>(type: "boolean", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListItem", x => x.ListItemId);
                    table.ForeignKey(
                        name: "FK_ListItem_ListItemCategory_ListItemCategoryId",
                        column: x => x.ListItemCategoryId,
                        principalTable: "ListItemCategory",
                        principalColumn: "ListItemCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectName = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ProjectSlug = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProjectMarketListItemId = table.Column<int>(type: "integer", nullable: false),
                    ProjectDescription = table.Column<string>(type: "text", nullable: true),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_Project_ListItem_ProjectMarketListItemId",
                        column: x => x.ProjectMarketListItemId,
                        principalTable: "ListItem",
                        principalColumn: "ListItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMember",
                columns: table => new
                {
                    ProjectMemberId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    ProjectRoleListItemId = table.Column<int>(type: "integer", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMember", x => x.ProjectMemberId);
                    table.ForeignKey(
                        name: "FK_ProjectMember_ListItem_ProjectRoleListItemId",
                        column: x => x.ProjectRoleListItemId,
                        principalTable: "ListItem",
                        principalColumn: "ListItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMember_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMember_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListItem_ListItemCategoryId_ListItemName",
                table: "ListItem",
                columns: new[] { "ListItemCategoryId", "ListItemName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListItem_ListItemSystemName",
                table: "ListItem",
                column: "ListItemSystemName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListItemCategory_ListItemCategorySystemName",
                table: "ListItemCategory",
                column: "ListItemCategorySystemName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Person_Email",
                table: "Person",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Person_UserName",
                table: "Person",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectMarketListItemId",
                table: "Project",
                column: "ProjectMarketListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectName",
                table: "Project",
                column: "ProjectName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectSlug",
                table: "Project",
                column: "ProjectSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMember_PersonId",
                table: "ProjectMember",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMember_ProjectId_PersonId",
                table: "ProjectMember",
                columns: new[] { "ProjectId", "PersonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMember_ProjectRoleListItemId",
                table: "ProjectMember",
                column: "ProjectRoleListItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectMember");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "ListItem");

            migrationBuilder.DropTable(
                name: "ListItemCategory");
        }
    }
}
