using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations.core
{
    public partial class UserRole_Added_on_PersonTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserRoleListItemId",
                table: "Person",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Person_UserRoleListItemId",
                table: "Person",
                column: "UserRoleListItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_ListItem_UserRoleListItemId",
                table: "Person",
                column: "UserRoleListItemId",
                principalTable: "ListItem",
                principalColumn: "ListItemId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Person_ListItem_UserRoleListItemId",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Person_UserRoleListItemId",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "UserRoleListItemId",
                table: "Person");
        }
    }
}
