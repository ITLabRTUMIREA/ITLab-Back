using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations.Postgres
{
    public partial class EquipmentGranter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GranterId",
                table: "EquipmentOwnerChanges",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentOwnerChanges_GranterId",
                table: "EquipmentOwnerChanges",
                column: "GranterId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentOwnerChanges_AspNetUsers_GranterId",
                table: "EquipmentOwnerChanges",
                column: "GranterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentOwnerChanges_AspNetUsers_GranterId",
                table: "EquipmentOwnerChanges");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentOwnerChanges_GranterId",
                table: "EquipmentOwnerChanges");

            migrationBuilder.DropColumn(
                name: "GranterId",
                table: "EquipmentOwnerChanges");
        }
    }
}
