using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class Modified_TableTestRunHistory_StepHistory_Added_timeSpanToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalTimeSpent",
                table: "TestRunTestCaseStepHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalTimeSpent",
                table: "TestRunTestCaseHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalTimeSpent",
                table: "TestRunHistory",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalTimeSpent",
                table: "TestRunTestCaseStepHistory");

            migrationBuilder.DropColumn(
                name: "TotalTimeSpent",
                table: "TestRunTestCaseHistory");

            migrationBuilder.DropColumn(
                name: "TotalTimeSpent",
                table: "TestRunHistory");
        }
    }
}
