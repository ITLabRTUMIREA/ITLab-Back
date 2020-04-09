using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations.Postgres
{
    public partial class Test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Equipments_SerialNumber",
                table: "Equipments",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentTypeId_Number",
                table: "Equipments",
                columns: new[] { "EquipmentTypeId", "Number" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Equipments_SerialNumber",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_EquipmentTypeId_Number",
                table: "Equipments");
        }
    }
}
