using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations.core
{
    public partial class Modified_Table_TestPlan_added_Unique_ProjectId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestPlan_ParentTestPlanId_TestPlanName",
                table: "TestPlan");

            migrationBuilder.CreateIndex(
                name: "IX_TestPlan_ParentTestPlanId_TestPlanName_ProjectId",
                table: "TestPlan",
                columns: new[] { "ParentTestPlanId", "TestPlanName", "ProjectId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestPlan_ParentTestPlanId_TestPlanName_ProjectId",
                table: "TestPlan");

            migrationBuilder.CreateIndex(
                name: "IX_TestPlan_ParentTestPlanId_TestPlanName",
                table: "TestPlan",
                columns: new[] { "ParentTestPlanId", "TestPlanName" },
                unique: true);
        }
    }
}
