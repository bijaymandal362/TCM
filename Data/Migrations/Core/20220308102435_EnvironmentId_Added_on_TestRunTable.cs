using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class EnvironmentId_Added_on_TestRunTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRun_ListItem_EnvironmentListItemId",
                table: "TestRun");

            migrationBuilder.RenameColumn(
                name: "EnvironmentListItemId",
                table: "TestRun",
                newName: "EnvironmentId");

            migrationBuilder.RenameIndex(
                name: "IX_TestRun_EnvironmentListItemId",
                table: "TestRun",
                newName: "IX_TestRun_EnvironmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRun_Environment_EnvironmentId",
                table: "TestRun",
                column: "EnvironmentId",
                principalTable: "Environment",
                principalColumn: "EnvironmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRun_Environment_EnvironmentId",
                table: "TestRun");

            migrationBuilder.RenameColumn(
                name: "EnvironmentId",
                table: "TestRun",
                newName: "EnvironmentListItemId");

            migrationBuilder.RenameIndex(
                name: "IX_TestRun_EnvironmentId",
                table: "TestRun",
                newName: "IX_TestRun_EnvironmentListItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRun_ListItem_EnvironmentListItemId",
                table: "TestRun",
                column: "EnvironmentListItemId",
                principalTable: "ListItem",
                principalColumn: "ListItemId");
        }
    }
}
