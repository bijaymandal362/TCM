using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations.core
{
    public partial class Added_Table_TestPlan_TestPlanTestCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestPlan",
                columns: table => new
                {
                    TestPlanId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentTestPlanId = table.Column<int>(type: "integer", nullable: true),
                    TestPlanName = table.Column<string>(type: "text", nullable: false),
                    OrderDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestPlan", x => x.TestPlanId);
                });

            migrationBuilder.CreateTable(
                name: "TestPlanTestCase",
                columns: table => new
                {
                    TestPlanTestCaseId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestPlanId = table.Column<int>(type: "integer", nullable: false),
                    ProjectModuleId = table.Column<int>(type: "integer", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestPlanTestCase", x => x.TestPlanTestCaseId);
                    table.ForeignKey(
                        name: "FK_TestPlanTestCase_ProjectModule_ProjectModuleId",
                        column: x => x.ProjectModuleId,
                        principalTable: "ProjectModule",
                        principalColumn: "ProjectModuleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestPlanTestCase_TestPlan_TestPlanId",
                        column: x => x.TestPlanId,
                        principalTable: "TestPlan",
                        principalColumn: "TestPlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestPlan_ParentTestPlanId_TestPlanName",
                table: "TestPlan",
                columns: new[] { "ParentTestPlanId", "TestPlanName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestPlanTestCase_ProjectModuleId",
                table: "TestPlanTestCase",
                column: "ProjectModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TestPlanTestCase_TestPlanId_ProjectModuleId",
                table: "TestPlanTestCase",
                columns: new[] { "TestPlanId", "ProjectModuleId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestPlanTestCase");

            migrationBuilder.DropTable(
                name: "TestPlan");
        }
    }
}
