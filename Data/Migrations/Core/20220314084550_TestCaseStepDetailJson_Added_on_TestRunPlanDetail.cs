using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class TestCaseStepDetailJson_Added_on_TestRunPlanDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestCaseStepDetailJson",
                table: "TestRunPlanDetail",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestCaseStepDetailJson",
                table: "TestRunPlanDetail");
        }
    }
}
