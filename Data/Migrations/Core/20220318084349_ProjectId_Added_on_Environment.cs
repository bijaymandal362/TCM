using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class ProjectId_Added_on_Environment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Environment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Environment_ProjectId",
                table: "Environment",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Environment_Project_ProjectId",
                table: "Environment",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Environment_Project_ProjectId",
                table: "Environment");

            migrationBuilder.DropIndex(
                name: "IX_Environment_ProjectId",
                table: "Environment");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Environment");
        }
    }
}
