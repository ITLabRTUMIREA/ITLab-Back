using Microsoft.EntityFrameworkCore.Migrations;

namespace BackEnd.DataBase.Migrations.Production
{
    public partial class AddUniqueueAndRequiredField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InternalName",
                table: "UserPropertyTypes",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyTypes_InternalName",
                table: "UserPropertyTypes",
                column: "InternalName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserPropertyTypes_InternalName",
                table: "UserPropertyTypes");

            migrationBuilder.AlterColumn<string>(
                name: "InternalName",
                table: "UserPropertyTypes",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
