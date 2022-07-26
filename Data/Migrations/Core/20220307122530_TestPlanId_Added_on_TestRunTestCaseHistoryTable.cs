using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class TestPlanId_Added_on_TestRunTestCaseHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestPlanId",
                table: "TestRunTestCaseHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestRunTestCaseHistory_TestPlanId",
                table: "TestRunTestCaseHistory",
                column: "TestPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRunTestCaseHistory_TestPlan_TestPlanId",
                table: "TestRunTestCaseHistory",
                column: "TestPlanId",
                principalTable: "TestPlan",
                principalColumn: "TestPlanId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRunTestCaseHistory_TestPlan_TestPlanId",
                table: "TestRunTestCaseHistory");

            migrationBuilder.DropIndex(
                name: "IX_TestRunTestCaseHistory_TestPlanId",
                table: "TestRunTestCaseHistory");

            migrationBuilder.DropColumn(
                name: "TestPlanId",
                table: "TestRunTestCaseHistory");
        }
    }
}
