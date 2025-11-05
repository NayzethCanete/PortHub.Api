using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixModelNullWarnings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Airlines_AirlineId",
                table: "Slots");

            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Gates_GateId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slot_Runway_ScheduledTime",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_AirlineId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Gates_GateName",
                table: "Gates");

            migrationBuilder.DropIndex(
                name: "IX_Airlines_ApiKey",
                table: "Airlines");

            migrationBuilder.DropIndex(
                name: "IX_Airlines_Code",
                table: "Airlines");

            migrationBuilder.DeleteData(
                table: "Airlines",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "AirlineId",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "FlightNumber",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "Available",
                table: "Gates");

            migrationBuilder.DropColumn(
                name: "GateName",
                table: "Gates");

            migrationBuilder.RenameColumn(
                name: "ScheduledTime",
                table: "Slots",
                newName: "ScheduleTime");

            migrationBuilder.RenameColumn(
                name: "GateId",
                table: "Gates",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Slots",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Runway",
                table: "Slots",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<int>(
                name: "FlightId",
                table: "Slots",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Flight_id",
                table: "Slots",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gate_id",
                table: "Slots",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Gates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Gates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Gates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "Validation",
                table: "Boardings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Airlines",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Airlines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Airlines",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<string>(
                name: "BaseAddress",
                table: "Airlines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ApiUrl",
                table: "Airlines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ApiKey",
                table: "Airlines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    FlightId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AirlineId = table.Column<int>(type: "int", nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SlotId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.FlightId);
                    table.ForeignKey(
                        name: "FK_Flights_Airlines_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "Airlines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightId = table.Column<int>(type: "int", nullable: false),
                    PassengerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "FlightId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Slots_FlightId",
                table: "Slots",
                column: "FlightId",
                unique: true,
                filter: "[FlightId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Boardings_TicketId",
                table: "Boardings",
                column: "TicketId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flights_AirlineId",
                table: "Flights",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_FlightId",
                table: "Tickets",
                column: "FlightId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boardings_Tickets_TicketId",
                table: "Boardings",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Flights_FlightId",
                table: "Slots",
                column: "FlightId",
                principalTable: "Flights",
                principalColumn: "FlightId");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Gates_GateId",
                table: "Slots",
                column: "GateId",
                principalTable: "Gates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boardings_Tickets_TicketId",
                table: "Boardings");

            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Flights_FlightId",
                table: "Slots");

            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Gates_GateId",
                table: "Slots");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Slots_FlightId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Boardings_TicketId",
                table: "Boardings");

            migrationBuilder.DropColumn(
                name: "FlightId",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "Flight_id",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "Gate_id",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Gates");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Gates");

            migrationBuilder.RenameColumn(
                name: "ScheduleTime",
                table: "Slots",
                newName: "ScheduledTime");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Gates",
                newName: "GateId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Slots",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Runway",
                table: "Slots",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AirlineId",
                table: "Slots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FlightNumber",
                table: "Slots",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Gates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "Gates",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "GateName",
                table: "Gates",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "Validation",
                table: "Boardings",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Airlines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Airlines",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Airlines",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BaseAddress",
                table: "Airlines",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApiUrl",
                table: "Airlines",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApiKey",
                table: "Airlines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Airlines",
                columns: new[] { "Id", "ApiKey", "ApiUrl", "BaseAddress", "Code", "Country", "Name" },
                values: new object[] { 1, "AS_DEV_KEY_123456789ABCDEF", "https://localhost:7001/api", "Buenos Aires", "AS", "Argentina", "AeroSol" });

            migrationBuilder.CreateIndex(
                name: "IX_Slot_Runway_ScheduledTime",
                table: "Slots",
                columns: new[] { "Runway", "ScheduledTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slots_AirlineId",
                table: "Slots",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_Gates_GateName",
                table: "Gates",
                column: "GateName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Airlines_ApiKey",
                table: "Airlines",
                column: "ApiKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Airlines_Code",
                table: "Airlines",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Airlines_AirlineId",
                table: "Slots",
                column: "AirlineId",
                principalTable: "Airlines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Gates_GateId",
                table: "Slots",
                column: "GateId",
                principalTable: "Gates",
                principalColumn: "GateId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
