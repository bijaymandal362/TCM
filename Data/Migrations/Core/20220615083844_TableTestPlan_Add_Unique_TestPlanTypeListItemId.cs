using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class TableTestPlan_Add_Unique_TestPlanTypeListItemId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestPlan_ParentTestPlanId_TestPlanName_ProjectId",
                table: "TestPlan");

            migrationBuilder.CreateIndex(
                name: "IX_TestPlan_ParentTestPlanId_TestPlanName_ProjectId_TestPlanTy~",
                table: "TestPlan",
                columns: new[] { "ParentTestPlanId", "TestPlanName", "ProjectId", "TestPlanTypeListItemId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestPlan_ParentTestPlanId_TestPlanName_ProjectId_TestPlanTy~",
                table: "TestPlan");

            migrationBuilder.CreateIndex(
                name: "IX_TestPlan_ParentTestPlanId_TestPlanName_ProjectId",
                table: "TestPlan",
                columns: new[] { "ParentTestPlanId", "TestPlanName", "ProjectId" },
                unique: true);
        }
    }
}
