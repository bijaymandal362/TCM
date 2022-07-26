using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class EnvironemtName_and_ProjectId_Unique_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Environment_EnvironmentName",
                table: "Environment");

            migrationBuilder.CreateIndex(
                name: "IX_Environment_EnvironmentName_ProjectId",
                table: "Environment",
                columns: new[] { "EnvironmentName", "ProjectId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Environment_EnvironmentName_ProjectId",
                table: "Environment");

            migrationBuilder.CreateIndex(
                name: "IX_Environment_EnvironmentName",
                table: "Environment",
                column: "EnvironmentName",
                unique: true);
        }
    }
}
