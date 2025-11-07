using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Validation",
                table: "Boardings");

            migrationBuilder.RenameColumn(
                name: "AccessTime",
                table: "Boardings",
                newName: "BoardingTime");

            migrationBuilder.AddColumn<string>(
                name: "FlightCode",
                table: "Boardings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GateId",
                table: "Boardings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PassengerName",
                table: "Boardings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Seat",
                table: "Boardings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Boardings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Boardings_GateId",
                table: "Boardings",
                column: "GateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boardings_Gates_GateId",
                table: "Boardings",
                column: "GateId",
                principalTable: "Gates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boardings_Gates_GateId",
                table: "Boardings");

            migrationBuilder.DropIndex(
                name: "IX_Boardings_GateId",
                table: "Boardings");

            migrationBuilder.DropColumn(
                name: "FlightCode",
                table: "Boardings");

            migrationBuilder.DropColumn(
                name: "GateId",
                table: "Boardings");

            migrationBuilder.DropColumn(
                name: "PassengerName",
                table: "Boardings");

            migrationBuilder.DropColumn(
                name: "Seat",
                table: "Boardings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Boardings");

            migrationBuilder.RenameColumn(
                name: "BoardingTime",
                table: "Boardings",
                newName: "AccessTime");

            migrationBuilder.AddColumn<bool>(
                name: "Validation",
                table: "Boardings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
