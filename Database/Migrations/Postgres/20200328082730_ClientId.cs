using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations.Postgres
{
    public partial class ClientId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Shift",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Place",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Shift");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Place");
        }
    }
}
