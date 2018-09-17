using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BackEnd.DataBase.Migrations.Production
{
    public partial class Descriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Shift",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Place",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "EquipmentTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Equipments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypes_ParentId",
                table: "EquipmentTypes",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTypes_EquipmentTypes_ParentId",
                table: "EquipmentTypes",
                column: "ParentId",
                principalTable: "EquipmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTypes_EquipmentTypes_ParentId",
                table: "EquipmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentTypes_ParentId",
                table: "EquipmentTypes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Shift");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Place");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "EquipmentTypes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Equipments");
        }
    }
}
