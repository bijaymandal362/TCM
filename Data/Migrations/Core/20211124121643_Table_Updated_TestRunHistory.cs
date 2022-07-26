using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class Table_Updated_TestRunHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRunHistory_ListItem_TestRunStatusListItemId",
                table: "TestRunHistory");

            migrationBuilder.DropIndex(
                name: "IX_TestRunHistory_TestRunStatusListItemId",
                table: "TestRunHistory");

            migrationBuilder.DropColumn(
                name: "TestRunStatusListItemId",
                table: "TestRunHistory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestRunStatusListItemId",
                table: "TestRunHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestRunHistory_TestRunStatusListItemId",
                table: "TestRunHistory",
                column: "TestRunStatusListItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRunHistory_ListItem_TestRunStatusListItemId",
                table: "TestRunHistory",
                column: "TestRunStatusListItemId",
                principalTable: "ListItem",
                principalColumn: "ListItemId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
