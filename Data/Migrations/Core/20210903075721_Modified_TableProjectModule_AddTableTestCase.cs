using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations.core
{
    public partial class Modified_TableProjectModule_AddTableTestCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProjectModule",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProjectModule");
        }
    }
}
