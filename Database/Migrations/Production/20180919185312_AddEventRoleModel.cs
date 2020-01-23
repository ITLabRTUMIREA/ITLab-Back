using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBase.Migrations.Production
{
    public partial class AddEventRoleModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlaceUserEventRole",
                columns: table => new
                {
                    PlaceId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    EventRoleId = table.Column<Guid>(nullable: false),
                    UserStatus = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    DoneTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceUserEventRole", x => new { x.UserId, x.PlaceId, x.EventRoleId });
                    table.ForeignKey(
                        name: "FK_PlaceUserEventRole_EventRoles_EventRoleId",
                        column: x => x.EventRoleId,
                        principalTable: "EventRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaceUserEventRole_Place_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Place",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaceUserEventRole_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaceUserEventRole_EventRoleId",
                table: "PlaceUserEventRole",
                column: "EventRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceUserEventRole_PlaceId",
                table: "PlaceUserEventRole",
                column: "PlaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaceUserEventRole");

            migrationBuilder.DropTable(
                name: "EventRoles");
        }
    }
}
