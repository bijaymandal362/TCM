using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations.core
{
    public partial class ModifiedTable_ProjectModule_TestCaseDetails_TestCaseStep : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestCaseListItemId",
                table: "TestCaseStepDetail",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TestCaseListItemId",
                table: "TestCaseDetail",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OrderDate",
                table: "ProjectModule",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseStepDetail_TestCaseListItemId",
                table: "TestCaseStepDetail",
                column: "TestCaseListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseDetail_TestCaseListItemId",
                table: "TestCaseDetail",
                column: "TestCaseListItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCaseDetail_ListItem_TestCaseListItemId",
                table: "TestCaseDetail",
                column: "TestCaseListItemId",
                principalTable: "ListItem",
                principalColumn: "ListItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestCaseStepDetail_ListItem_TestCaseListItemId",
                table: "TestCaseStepDetail",
                column: "TestCaseListItemId",
                principalTable: "ListItem",
                principalColumn: "ListItemId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCaseDetail_ListItem_TestCaseListItemId",
                table: "TestCaseDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCaseStepDetail_ListItem_TestCaseListItemId",
                table: "TestCaseStepDetail");

            migrationBuilder.DropIndex(
                name: "IX_TestCaseStepDetail_TestCaseListItemId",
                table: "TestCaseStepDetail");

            migrationBuilder.DropIndex(
                name: "IX_TestCaseDetail_TestCaseListItemId",
                table: "TestCaseDetail");

            migrationBuilder.DropColumn(
                name: "TestCaseListItemId",
                table: "TestCaseStepDetail");

            migrationBuilder.DropColumn(
                name: "TestCaseListItemId",
                table: "TestCaseDetail");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "ProjectModule");
        }
    }
}
