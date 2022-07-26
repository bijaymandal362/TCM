using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class Modified_TableTestRunHistory_timeSpanToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "TotalTimeSpent",
                table: "TestRunTestCaseStepHistory",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TotalTimeSpent",
                table: "TestRunTestCaseHistory",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TotalTimeSpent",
                table: "TestRunHistory",
                type: "interval",
                nullable: true);
        }
    }
}
