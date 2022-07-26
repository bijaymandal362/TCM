using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations.core
{
    public partial class Modified_Table_TestPlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestPlanTypeListItemId",
                table: "TestPlan",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestPlan_TestPlanTypeListItemId",
                table: "TestPlan",
                column: "TestPlanTypeListItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestPlan_ListItem_TestPlanTypeListItemId",
                table: "TestPlan",
                column: "TestPlanTypeListItemId",
                principalTable: "ListItem",
                principalColumn: "ListItemId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestPlan_ListItem_TestPlanTypeListItemId",
                table: "TestPlan");

            migrationBuilder.DropIndex(
                name: "IX_TestPlan_TestPlanTypeListItemId",
                table: "TestPlan");

            migrationBuilder.DropColumn(
                name: "TestPlanTypeListItemId",
                table: "TestPlan");
        }
    }
}
