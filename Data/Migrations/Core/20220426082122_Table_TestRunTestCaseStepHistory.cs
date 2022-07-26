using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations.core
{
    public partial class Table_TestRunTestCaseStepHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestRunTestCaseStepHistoryDocument",
                columns: table => new
                {
                    TestRunTestCaseStepHistoryDocumentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestRunTestCaseStepHistoryId = table.Column<int>(type: "integer", nullable: false),
                    DocumentId = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    InsertPersonId = table.Column<int>(type: "integer", nullable: false),
                    InsertDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatePersonId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRunTestCaseStepHistoryDocument", x => x.TestRunTestCaseStepHistoryDocumentId);
                    table.ForeignKey(
                        name: "FK_TestRunTestCaseStepHistoryDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestRunTestCaseStepHistoryDocument_TestRunTestCaseStepHisto~",
                        column: x => x.TestRunTestCaseStepHistoryId,
                        principalTable: "TestRunTestCaseStepHistory",
                        principalColumn: "TestRunTestCaseStepHistoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestRunTestCaseStepHistoryDocument_DocumentId",
                table: "TestRunTestCaseStepHistoryDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunTestCaseStepHistoryDocument_TestRunTestCaseStepHisto~",
                table: "TestRunTestCaseStepHistoryDocument",
                columns: new[] { "TestRunTestCaseStepHistoryId", "DocumentId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestRunTestCaseStepHistoryDocument");
        }
    }
}
