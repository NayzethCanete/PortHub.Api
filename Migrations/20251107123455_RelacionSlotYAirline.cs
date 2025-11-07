using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class RelacionSlotYAirline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AirlineId",
                table: "Slots",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slots_AirlineId",
                table: "Slots",
                column: "AirlineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Airlines_AirlineId",
                table: "Slots",
                column: "AirlineId",
                principalTable: "Airlines",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Airlines_AirlineId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_AirlineId",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "AirlineId",
                table: "Slots");
        }
    }
}
