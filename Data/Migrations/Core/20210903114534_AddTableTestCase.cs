using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations.core
{
    public partial class AddTableTestCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestCaseDetail",
                columns: table => new
                {
                    TestCaseDetailId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectModuleId = table.Column<int>(type: "integer", nullable: false),
                    PreCondition = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ExpectedResult = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    TestCases = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseDetail", x => x.TestCaseDetailId);
                    table.ForeignKey(
                        name: "FK_TestCaseDetail_ProjectModule_ProjectModuleId",
                        column: x => x.ProjectModuleId,
                        principalTable: "ProjectModule",
                        principalColumn: "ProjectModuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseDetail_ProjectModuleId",
                table: "TestCaseDetail",
                column: "ProjectModuleId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestCaseDetail");
        }
    }
}
