using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations.core
{
    public partial class Nullable_MarketId_On_PersonTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Person_ListItem_UserMarketListItemId",
                table: "Person");

            migrationBuilder.AlterColumn<int>(
                name: "UserMarketListItemId",
                table: "Person",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_ListItem_UserMarketListItemId",
                table: "Person",
                column: "UserMarketListItemId",
                principalTable: "ListItem",
                principalColumn: "ListItemId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Person_ListItem_UserMarketListItemId",
                table: "Person");

            migrationBuilder.AlterColumn<int>(
                name: "UserMarketListItemId",
                table: "Person",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Person_ListItem_UserMarketListItemId",
                table: "Person",
                column: "UserMarketListItemId",
                principalTable: "ListItem",
                principalColumn: "ListItemId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
