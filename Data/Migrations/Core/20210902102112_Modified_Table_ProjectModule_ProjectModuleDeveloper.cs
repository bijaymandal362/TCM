using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations.core
{
    public partial class Modified_Table_ProjectModule_ProjectModuleDeveloper : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectModule",
                columns: table => new
                {
                    ProjectModuleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentProjectModuleId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    ModuleName = table.Column<string>(type: "text", nullable: false),
                    ProjectModuleListItemId = table.Column<int>(type: "integer", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectModule", x => x.ProjectModuleId);
                    table.ForeignKey(
                        name: "FK_ProjectModule_ListItem_ProjectModuleListItemId",
                        column: x => x.ProjectModuleListItemId,
                        principalTable: "ListItem",
                        principalColumn: "ListItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectModule_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectModuleDeveloper",
                columns: table => new
                {
                    ProjectModuleDeveloperId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectModuleId = table.Column<int>(type: "integer", nullable: false),
                    ProjectMemberId = table.Column<int>(type: "integer", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectModuleDeveloper", x => x.ProjectModuleDeveloperId);
                    table.ForeignKey(
                        name: "FK_ProjectModuleDeveloper_ProjectMember_ProjectMemberId",
                        column: x => x.ProjectMemberId,
                        principalTable: "ProjectMember",
                        principalColumn: "ProjectMemberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectModuleDeveloper_ProjectModule_ProjectModuleId",
                        column: x => x.ProjectModuleId,
                        principalTable: "ProjectModule",
                        principalColumn: "ProjectModuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectModule_ProjectId_ModuleName_ParentProjectModuleId",
                table: "ProjectModule",
                columns: new[] { "ProjectId", "ModuleName", "ParentProjectModuleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectModule_ProjectModuleListItemId",
                table: "ProjectModule",
                column: "ProjectModuleListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectModuleDeveloper_ProjectMemberId",
                table: "ProjectModuleDeveloper",
                column: "ProjectMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectModuleDeveloper_ProjectModuleId_ProjectMemberId",
                table: "ProjectModuleDeveloper",
                columns: new[] { "ProjectModuleId", "ProjectMemberId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectModuleDeveloper");

            migrationBuilder.DropTable(
                name: "ProjectModule");
        }
    }
}
