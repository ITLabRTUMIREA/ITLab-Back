using Microsoft.EntityFrameworkCore.Migrations;

namespace BackEnd.DataBase.Migrations.Production
{
    public partial class UpdateUserPropertyType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaceUserEventRole_EventRoles_EventRoleId",
                table: "PlaceUserEventRole");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "UserPropertyTypes",
                newName: "InternalName");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaceUserEventRole_EventRoles_EventRoleId",
                table: "PlaceUserEventRole",
                column: "EventRoleId",
                principalTable: "EventRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaceUserEventRole_EventRoles_EventRoleId",
                table: "PlaceUserEventRole");

            migrationBuilder.RenameColumn(
                name: "InternalName",
                table: "UserPropertyTypes",
                newName: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaceUserEventRole_EventRoles_EventRoleId",
                table: "PlaceUserEventRole",
                column: "EventRoleId",
                principalTable: "EventRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
