using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations.core
{
    public partial class TestCaseStepDetailTable_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestCases",
                table: "TestCaseDetail");

            migrationBuilder.CreateTable(
                name: "TestCaseStepDetail",
                columns: table => new
                {
                    TestCaseStepDetailId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StepNumber = table.Column<int>(type: "integer", nullable: false),
                    StepDescription = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ExpectedResult = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ProjectModuleId = table.Column<int>(type: "integer", nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseStepDetail", x => x.TestCaseStepDetailId);
                    table.ForeignKey(
                        name: "FK_TestCaseStepDetail_ProjectModule_ProjectModuleId",
                        column: x => x.ProjectModuleId,
                        principalTable: "ProjectModule",
                        principalColumn: "ProjectModuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseStepDetail_ProjectModuleId",
                table: "TestCaseStepDetail",
                column: "ProjectModuleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestCaseStepDetail");

            migrationBuilder.AddColumn<string>(
                name: "TestCases",
                table: "TestCaseDetail",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}
