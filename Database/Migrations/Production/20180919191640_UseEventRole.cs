using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBase.Migrations.Production
{
    public partial class UseEventRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaceUserRole");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlaceUserRole",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    PlaceId = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    DoneTime = table.Column<DateTime>(nullable: true),
                    RoleId = table.Column<Guid>(nullable: false),
                    UserStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceUserRole", x => new { x.UserId, x.PlaceId });
                    table.ForeignKey(
                        name: "FK_PlaceUserRole_Place_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Place",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaceUserRole_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaceUserRole_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaceUserRole_PlaceId",
                table: "PlaceUserRole",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceUserRole_RoleId",
                table: "PlaceUserRole",
                column: "RoleId");
        }
    }
}
