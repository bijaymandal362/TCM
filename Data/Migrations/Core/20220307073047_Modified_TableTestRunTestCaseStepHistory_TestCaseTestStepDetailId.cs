using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class Modified_TableTestRunTestCaseStepHistory_TestCaseTestStepDetailId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRunTestCaseStepHistory_TestCaseDetail_TestCaseDetailId",
                table: "TestRunTestCaseStepHistory");

            migrationBuilder.RenameColumn(
                name: "TestCaseDetailId",
                table: "TestRunTestCaseStepHistory",
                newName: "TestCaseStepDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_TestRunTestCaseStepHistory_TestCaseDetailId",
                table: "TestRunTestCaseStepHistory",
                newName: "IX_TestRunTestCaseStepHistory_TestCaseStepDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRunTestCaseStepHistory_TestCaseStepDetail_TestCaseStepD~",
                table: "TestRunTestCaseStepHistory",
                column: "TestCaseStepDetailId",
                principalTable: "TestCaseStepDetail",
                principalColumn: "TestCaseStepDetailId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRunTestCaseStepHistory_TestCaseStepDetail_TestCaseStepD~",
                table: "TestRunTestCaseStepHistory");

            migrationBuilder.RenameColumn(
                name: "TestCaseStepDetailId",
                table: "TestRunTestCaseStepHistory",
                newName: "TestCaseDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_TestRunTestCaseStepHistory_TestCaseStepDetailId",
                table: "TestRunTestCaseStepHistory",
                newName: "IX_TestRunTestCaseStepHistory_TestCaseDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRunTestCaseStepHistory_TestCaseDetail_TestCaseDetailId",
                table: "TestRunTestCaseStepHistory",
                column: "TestCaseDetailId",
                principalTable: "TestCaseDetail",
                principalColumn: "TestCaseDetailId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
