using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BackEnd.DataBase.Migrations.Production
{
    public partial class EquipmentOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Equipments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_OwnerId",
                table: "Equipments",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_AspNetUsers_OwnerId",
                table: "Equipments",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_AspNetUsers_OwnerId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_OwnerId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Equipments");

            migrationBuilder.AddColumn<string>(
                name: "StudentID",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }
    }
}
