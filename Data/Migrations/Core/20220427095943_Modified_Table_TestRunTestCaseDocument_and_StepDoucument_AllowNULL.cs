using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class Modified_Table_TestRunTestCaseDocument_and_StepDoucument_AllowNULL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRunTestCaseHistoryDocument_Document_DocumentId",
                table: "TestRunTestCaseHistoryDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRunTestCaseStepHistoryDocument_Document_DocumentId",
                table: "TestRunTestCaseStepHistoryDocument");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentId",
                table: "TestRunTestCaseStepHistoryDocument",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentId",
                table: "TestRunTestCaseHistoryDocument",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRunTestCaseHistoryDocument_Document_DocumentId",
                table: "TestRunTestCaseHistoryDocument",
                column: "DocumentId",
                principalTable: "Document",
                principalColumn: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRunTestCaseStepHistoryDocument_Document_DocumentId",
                table: "TestRunTestCaseStepHistoryDocument",
                column: "DocumentId",
                principalTable: "Document",
                principalColumn: "DocumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRunTestCaseHistoryDocument_Document_DocumentId",
                table: "TestRunTestCaseHistoryDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRunTestCaseStepHistoryDocument_Document_DocumentId",
                table: "TestRunTestCaseStepHistoryDocument");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentId",
                table: "TestRunTestCaseStepHistoryDocument",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DocumentId",
                table: "TestRunTestCaseHistoryDocument",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TestRunTestCaseHistoryDocument_Document_DocumentId",
                table: "TestRunTestCaseHistoryDocument",
                column: "DocumentId",
                principalTable: "Document",
                principalColumn: "DocumentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestRunTestCaseStepHistoryDocument_Document_DocumentId",
                table: "TestRunTestCaseStepHistoryDocument",
                column: "DocumentId",
                principalTable: "Document",
                principalColumn: "DocumentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
