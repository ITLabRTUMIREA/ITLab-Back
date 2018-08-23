using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BackEnd.DataBase.Migrations.Production
{
    public partial class TimeForPlaceUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "PlaceUserRole",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DoneTime",
                table: "PlaceUserRole",
                nullable: true);

            var nowTime = DateTime.UtcNow;
            migrationBuilder.UpdateData("PlaceUserRole", "CreateTime", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CreateTime", nowTime);
            migrationBuilder.UpdateData("PlaceUserRole", "DoneTime", null, "DoneTime", nowTime);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "PlaceUserRole");

            migrationBuilder.DropColumn(
                name: "DoneTime",
                table: "PlaceUserRole");
        }
    }
}
