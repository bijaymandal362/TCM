using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.core
{
    public partial class Modified_TableEnvironment_UniqueEnvironment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Environment_EnvironmentName",
                table: "Environment",
                column: "EnvironmentName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Environment_EnvironmentName",
                table: "Environment");
        }
    }
}
