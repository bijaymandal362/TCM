using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class Modified_Table_TestRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRun_ListItem_EnvironmentListItemId",
                table: "TestRun");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRun_ProjectMember_DefaultAssigneeProjectMemberId",
                table: "TestRun");

            migrationBuilder.AlterColumn<int>(
                name: "EnvironmentListItemId",
                table: "TestRun",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DefaultAssigneeProjectMemberId",
                table: "TestRun",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRun_ListItem_EnvironmentListItemId",
                table: "TestRun",
                column: "EnvironmentListItemId",
                principalTable: "ListItem",
                principalColumn: "ListItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRun_ProjectMember_DefaultAssigneeProjectMemberId",
                table: "TestRun",
                column: "DefaultAssigneeProjectMemberId",
                principalTable: "ProjectMember",
                principalColumn: "ProjectMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRun_ListItem_EnvironmentListItemId",
                table: "TestRun");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRun_ProjectMember_DefaultAssigneeProjectMemberId",
                table: "TestRun");

            migrationBuilder.AlterColumn<int>(
                name: "EnvironmentListItemId",
                table: "TestRun",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DefaultAssigneeProjectMemberId",
                table: "TestRun",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TestRun_ListItem_EnvironmentListItemId",
                table: "TestRun",
                column: "EnvironmentListItemId",
                principalTable: "ListItem",
                principalColumn: "ListItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestRun_ProjectMember_DefaultAssigneeProjectMemberId",
                table: "TestRun",
                column: "DefaultAssigneeProjectMemberId",
                principalTable: "ProjectMember",
                principalColumn: "ProjectMemberId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
