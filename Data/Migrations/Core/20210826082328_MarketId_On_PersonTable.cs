using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations.core
{
    public partial class MarketId_On_PersonTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserMarketListItemId",
                table: "Person",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Person_UserMarketListItemId",
                table: "Person",
                column: "UserMarketListItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_ListItem_UserMarketListItemId",
                table: "Person",
                column: "UserMarketListItemId",
                principalTable: "ListItem",
                principalColumn: "ListItemId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Person_ListItem_UserMarketListItemId",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Person_UserMarketListItemId",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "UserMarketListItemId",
                table: "Person");
        }
    }
}
