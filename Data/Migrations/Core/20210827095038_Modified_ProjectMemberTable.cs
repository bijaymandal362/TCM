using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations.core
{
    public partial class Modified_ProjectMemberTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMember_Person_InsertedPersonId",
                table: "ProjectMember");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMember_InsertedPersonId",
                table: "ProjectMember");

            migrationBuilder.DropColumn(
                name: "InsertedPersonId",
                table: "ProjectMember");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InsertedPersonId",
                table: "ProjectMember",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMember_InsertedPersonId",
                table: "ProjectMember",
                column: "InsertedPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMember_Person_InsertedPersonId",
                table: "ProjectMember",
                column: "InsertedPersonId",
                principalTable: "Person",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
