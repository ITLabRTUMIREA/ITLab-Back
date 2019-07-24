using Microsoft.EntityFrameworkCore.Migrations;

namespace BackEnd.DataBase.Migrations.Production
{
    public partial class UserPropertyTypesUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaceUserEventRole_EventRoles_EventRoleId",
                table: "PlaceUserEventRole");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "UserPropertyTypes",
                newName: "PublicName");

            migrationBuilder.AddColumn<string>(
                name: "InternalName",
                table: "UserPropertyTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyTypes_InternalName",
                table: "UserPropertyTypes",
                column: "InternalName",
                unique: true,
                filter: "[InternalName] IS NOT NULL");

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

            migrationBuilder.DropIndex(
                name: "IX_UserPropertyTypes_InternalName",
                table: "UserPropertyTypes");

            migrationBuilder.DropColumn(
                name: "InternalName",
                table: "UserPropertyTypes");

            migrationBuilder.RenameColumn(
                name: "PublicName",
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
