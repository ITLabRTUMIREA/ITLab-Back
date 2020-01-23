using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBase.Migrations.Production
{
    public partial class EuipmentUpgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlaceUserEventRole",
                table: "PlaceUserEventRole");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Events",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Deep",
                table: "EquipmentTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastNumber",
                table: "EquipmentTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RootId",
                table: "EquipmentTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortTitle",
                table: "EquipmentTypes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Equipments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Equipments",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlaceUserEventRole",
                table: "PlaceUserEventRole",
                columns: new[] { "UserId", "PlaceId" });

            migrationBuilder.CreateTable(
                name: "UserPropertyTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DefaultStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPropertyTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    UserPropertyTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProperties_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProperties_UserPropertyTypes_UserPropertyTypeId",
                        column: x => x.UserPropertyTypeId,
                        principalTable: "UserPropertyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypes_RootId",
                table: "EquipmentTypes",
                column: "RootId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_ParentId",
                table: "Equipments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProperties_UserId",
                table: "UserProperties",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProperties_UserPropertyTypeId",
                table: "UserProperties",
                column: "UserPropertyTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Equipments_ParentId",
                table: "Equipments",
                column: "ParentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTypes_EquipmentTypes_RootId",
                table: "EquipmentTypes",
                column: "RootId",
                principalTable: "EquipmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Equipments_ParentId",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTypes_EquipmentTypes_RootId",
                table: "EquipmentTypes");

            migrationBuilder.DropTable(
                name: "UserProperties");

            migrationBuilder.DropTable(
                name: "UserPropertyTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlaceUserEventRole",
                table: "PlaceUserEventRole");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentTypes_RootId",
                table: "EquipmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_ParentId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Deep",
                table: "EquipmentTypes");

            migrationBuilder.DropColumn(
                name: "LastNumber",
                table: "EquipmentTypes");

            migrationBuilder.DropColumn(
                name: "RootId",
                table: "EquipmentTypes");

            migrationBuilder.DropColumn(
                name: "ShortTitle",
                table: "EquipmentTypes");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Equipments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlaceUserEventRole",
                table: "PlaceUserEventRole",
                columns: new[] { "UserId", "PlaceId", "EventRoleId" });
        }
    }
}
