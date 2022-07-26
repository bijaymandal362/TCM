using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations.core
{
    public partial class Modified_TestRunTestCaseHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRunTestCaseHistory_ProjectMember_AssigneeProjectMemberId",
                table: "TestRunTestCaseHistory");

            migrationBuilder.AlterColumn<int>(
                name: "AssigneeProjectMemberId",
                table: "TestRunTestCaseHistory",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRunTestCaseHistory_ProjectMember_AssigneeProjectMemberId",
                table: "TestRunTestCaseHistory",
                column: "AssigneeProjectMemberId",
                principalTable: "ProjectMember",
                principalColumn: "ProjectMemberId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRunTestCaseHistory_ProjectMember_AssigneeProjectMemberId",
                table: "TestRunTestCaseHistory");

            migrationBuilder.AlterColumn<int>(
                name: "AssigneeProjectMemberId",
                table: "TestRunTestCaseHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TestRunTestCaseHistory_ProjectMember_AssigneeProjectMemberId",
                table: "TestRunTestCaseHistory",
                column: "AssigneeProjectMemberId",
                principalTable: "ProjectMember",
                principalColumn: "ProjectMemberId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
