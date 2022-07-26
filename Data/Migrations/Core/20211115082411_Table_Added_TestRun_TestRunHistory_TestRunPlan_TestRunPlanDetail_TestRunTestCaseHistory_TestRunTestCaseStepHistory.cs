using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations.core
{
    public partial class Table_Added_TestRun_TestRunHistory_TestRunPlan_TestRunPlanDetail_TestRunTestCaseHistory_TestRunTestCaseStepHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestRun",
                columns: table => new
                {
                    TestRunId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    EnvironmentListItemId = table.Column<int>(type: "integer", nullable: false),
                    DefaultAssigneeProjectMemberId = table.Column<int>(type: "integer", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRun", x => x.TestRunId);
                    table.ForeignKey(
                        name: "FK_TestRun_ListItem_EnvironmentListItemId",
                        column: x => x.EnvironmentListItemId,
                        principalTable: "ListItem",
                        principalColumn: "ListItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestRun_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestRun_ProjectMember_DefaultAssigneeProjectMemberId",
                        column: x => x.DefaultAssigneeProjectMemberId,
                        principalTable: "ProjectMember",
                        principalColumn: "ProjectMemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestRunHistory",
                columns: table => new
                {
                    TestRunHistoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TotalTimeSpent = table.Column<TimeSpan>(type: "interval", nullable: false),
                    TestRunId = table.Column<int>(type: "integer", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRunHistory", x => x.TestRunHistoryId);
                    table.ForeignKey(
                        name: "FK_TestRunHistory_TestRun_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "TestRun",
                        principalColumn: "TestRunId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestRunPlan",
                columns: table => new
                {
                    TestRunPlanId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestRunId = table.Column<int>(type: "integer", nullable: false),
                    TestPlanId = table.Column<int>(type: "integer", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRunPlan", x => x.TestRunPlanId);
                    table.ForeignKey(
                        name: "FK_TestRunPlan_TestPlan_TestPlanId",
                        column: x => x.TestPlanId,
                        principalTable: "TestPlan",
                        principalColumn: "TestPlanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestRunPlan_TestRun_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "TestRun",
                        principalColumn: "TestRunId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestRunTestCaseHistory",
                columns: table => new
                {
                    TestRunTestCaseHistoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TotalTimeSpent = table.Column<TimeSpan>(type: "interval", nullable: false),
                    TestRunStatusListItemId = table.Column<int>(type: "integer", nullable: false),
                    AssigneeProjectMemberId = table.Column<int>(type: "integer", nullable: false),
                    TestRunHistoryId = table.Column<int>(type: "integer", nullable: false),
                    ProjectModuleId = table.Column<int>(type: "integer", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRunTestCaseHistory", x => x.TestRunTestCaseHistoryId);
                    table.ForeignKey(
                        name: "FK_TestRunTestCaseHistory_ListItem_TestRunStatusListItemId",
                        column: x => x.TestRunStatusListItemId,
                        principalTable: "ListItem",
                        principalColumn: "ListItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestRunTestCaseHistory_ProjectMember_AssigneeProjectMemberId",
                        column: x => x.AssigneeProjectMemberId,
                        principalTable: "ProjectMember",
                        principalColumn: "ProjectMemberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestRunTestCaseHistory_ProjectModule_ProjectModuleId",
                        column: x => x.ProjectModuleId,
                        principalTable: "ProjectModule",
                        principalColumn: "ProjectModuleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestRunTestCaseHistory_TestRunHistory_TestRunHistoryId",
                        column: x => x.TestRunHistoryId,
                        principalTable: "TestRunHistory",
                        principalColumn: "TestRunHistoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestRunPlanDetail",
                columns: table => new
                {
                    TestRunPlanDetailId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestRunPlanId = table.Column<int>(type: "integer", nullable: false),
                    TestPlanDetailJson = table.Column<string>(type: "text", nullable: true),
                    TestCaseDetailJson = table.Column<string>(type: "text", nullable: true),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRunPlanDetail", x => x.TestRunPlanDetailId);
                    table.ForeignKey(
                        name: "FK_TestRunPlanDetail_TestRunPlan_TestRunPlanId",
                        column: x => x.TestRunPlanId,
                        principalTable: "TestRunPlan",
                        principalColumn: "TestRunPlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestRunTestCaseStepHistory",
                columns: table => new
                {
                    TestRunTestCaseStepHistoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TotalTimeSpent = table.Column<TimeSpan>(type: "interval", nullable: false),
                    TestRunStatusListItemId = table.Column<int>(type: "integer", nullable: false),
                    TestCaseDetailId = table.Column<int>(type: "integer", nullable: false),
                    TestRunTestCaseHistoryId = table.Column<int>(type: "integer", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRunTestCaseStepHistory", x => x.TestRunTestCaseStepHistoryId);
                    table.ForeignKey(
                        name: "FK_TestRunTestCaseStepHistory_ListItem_TestRunStatusListItemId",
                        column: x => x.TestRunStatusListItemId,
                        principalTable: "ListItem",
                        principalColumn: "ListItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestRunTestCaseStepHistory_TestCaseDetail_TestCaseDetailId",
                        column: x => x.TestCaseDetailId,
                        principalTable: "TestCaseDetail",
                        principalColumn: "TestCaseDetailId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestRunTestCaseStepHistory_TestRunTestCaseHistory_TestRunTe~",
                        column: x => x.TestRunTestCaseHistoryId,
                        principalTable: "TestRunTestCaseHistory",
                        principalColumn: "TestRunTestCaseHistoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestRun_DefaultAssigneeProjectMemberId",
                table: "TestRun",
                column: "DefaultAssigneeProjectMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRun_EnvironmentListItemId",
                table: "TestRun",
                column: "EnvironmentListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRun_ProjectId",
                table: "TestRun",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunHistory_TestRunId",
                table: "TestRunHistory",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunPlan_TestPlanId",
                table: "TestRunPlan",
                column: "TestPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunPlan_TestRunId",
                table: "TestRunPlan",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunPlanDetail_TestRunPlanId",
                table: "TestRunPlanDetail",
                column: "TestRunPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunTestCaseHistory_AssigneeProjectMemberId",
                table: "TestRunTestCaseHistory",
                column: "AssigneeProjectMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunTestCaseHistory_ProjectModuleId",
                table: "TestRunTestCaseHistory",
                column: "ProjectModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunTestCaseHistory_TestRunHistoryId",
                table: "TestRunTestCaseHistory",
                column: "TestRunHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunTestCaseHistory_TestRunStatusListItemId",
                table: "TestRunTestCaseHistory",
                column: "TestRunStatusListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunTestCaseStepHistory_TestCaseDetailId",
                table: "TestRunTestCaseStepHistory",
                column: "TestCaseDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunTestCaseStepHistory_TestRunStatusListItemId",
                table: "TestRunTestCaseStepHistory",
                column: "TestRunStatusListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunTestCaseStepHistory_TestRunTestCaseHistoryId",
                table: "TestRunTestCaseStepHistory",
                column: "TestRunTestCaseHistoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestRunPlanDetail");

            migrationBuilder.DropTable(
                name: "TestRunTestCaseStepHistory");

            migrationBuilder.DropTable(
                name: "TestRunPlan");

            migrationBuilder.DropTable(
                name: "TestRunTestCaseHistory");

            migrationBuilder.DropTable(
                name: "TestRunHistory");

            migrationBuilder.DropTable(
                name: "TestRun");
        }
    }
}
