using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBase.Migrations.Production
{
    public partial class PublicNameForUserPropertyType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicName",
                table: "UserPropertyTypes",
                nullable: false,
                defaultValue: "Не определено");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicName",
                table: "UserPropertyTypes");
        }
    }
}
