using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class Modified_TableProjecMoudle_UniqueWithProjectModuleType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectModule_ProjectId_ModuleName_ParentProjectModuleId",
                table: "ProjectModule");

            migrationBuilder.AlterColumn<string>(
                name: "PreCondition",
                table: "TestCaseDetail",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectModule_ProjectId_ModuleName_ParentProjectModuleId_Pr~",
                table: "ProjectModule",
                columns: new[] { "ProjectId", "ModuleName", "ParentProjectModuleId", "ProjectModuleListItemId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectModule_ProjectId_ModuleName_ParentProjectModuleId_Pr~",
                table: "ProjectModule");

            migrationBuilder.AlterColumn<string>(
                name: "PreCondition",
                table: "TestCaseDetail",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectModule_ProjectId_ModuleName_ParentProjectModuleId",
                table: "ProjectModule",
                columns: new[] { "ProjectId", "ModuleName", "ParentProjectModuleId" },
                unique: true);
        }
    }
}
