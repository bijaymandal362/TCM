using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations.core
{
    public partial class ModifiedColumnInsertedPerson_ProjectMemeberTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMember_Person_InsertedPersonId",
                table: "ProjectMember");

            migrationBuilder.AlterColumn<int>(
                name: "InsertedPersonId",
                table: "ProjectMember",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMember_Person_InsertedPersonId",
                table: "ProjectMember",
                column: "InsertedPersonId",
                principalTable: "Person",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMember_Person_InsertedPersonId",
                table: "ProjectMember");

            migrationBuilder.AlterColumn<int>(
                name: "InsertedPersonId",
                table: "ProjectMember",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMember_Person_InsertedPersonId",
                table: "ProjectMember",
                column: "InsertedPersonId",
                principalTable: "Person",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
