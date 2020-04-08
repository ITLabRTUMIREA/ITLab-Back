using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations.Postgres
{
    public partial class EquipmentOwnerHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentOwnerChanges",
                columns: table => new
                {
                    EquipmentId = table.Column<Guid>(nullable: false),
                    ChangeOwnerTime = table.Column<DateTime>(nullable: false),
                    NewOwnerId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentOwnerChanges", x => new { x.EquipmentId, x.ChangeOwnerTime });
                    table.ForeignKey(
                        name: "FK_EquipmentOwnerChanges_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentOwnerChanges_AspNetUsers_NewOwnerId",
                        column: x => x.NewOwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentOwnerChanges_NewOwnerId",
                table: "EquipmentOwnerChanges",
                column: "NewOwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentOwnerChanges");
        }
    }
}
