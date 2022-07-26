using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations.core
{
    public partial class ProjectStarred_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectStarred",
                columns: table => new
                {
                    ProjectStarredId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStarred", x => x.ProjectStarredId);
                    table.ForeignKey(
                        name: "FK_ProjectStarred_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectStarred_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStarred_PersonId",
                table: "ProjectStarred",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStarred_ProjectId_PersonId",
                table: "ProjectStarred",
                columns: new[] { "ProjectId", "PersonId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectStarred");
        }
    }
}
