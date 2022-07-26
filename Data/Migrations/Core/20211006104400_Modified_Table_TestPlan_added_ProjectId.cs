using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations.core
{
    public partial class Modified_Table_TestPlan_added_ProjectId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "TestPlan",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestPlan_ProjectId",
                table: "TestPlan",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestPlan_Project_ProjectId",
                table: "TestPlan",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestPlan_Project_ProjectId",
                table: "TestPlan");

            migrationBuilder.DropIndex(
                name: "IX_TestPlan_ProjectId",
                table: "TestPlan");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "TestPlan");
        }
    }
}
