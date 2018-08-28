using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BackEnd.DataBase.Migrations.Production
{
    public partial class CreateToCreationTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "PlaceUserRole",
                newName: "CreationTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "PlaceUserRole",
                newName: "CreateTime");
        }
    }
}
