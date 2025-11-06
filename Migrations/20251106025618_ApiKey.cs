using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PortHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class ApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boardings_Slots_SlotId",
                table: "Boardings");

            migrationBuilder.DropForeignKey(
                name: "FK_Boardings_Tickets_TicketId",
                table: "Boardings");

            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Gates_GateId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Boardings_TicketId",
                table: "Boardings");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Tickets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Seat",
                table: "Tickets",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PassengerName",
                table: "Tickets",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FlightCode",
                table: "Tickets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FlightCode",
                table: "Slots",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Gates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Gates",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Airlines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Airlines",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
                values: new object[] { 1, "AR_KEY_123456789ABCDEF01234", "http://localhost:5241/api/airline", "Buenos Aires", "AR", "Argentina", "Aerolíneas Argentinas" });

            migrationBuilder.InsertData(
                table: "Gates",
                columns: new[] { "Id", "IsAvailable", "Location", "Name" },
                values: new object[,]
                {
                    { 1, true, "Terminal A - Norte", "A1" },
                    { 2, true, "Terminal A - Norte", "A2" },
                    { 3, true, "Terminal B - Sur", "B1" },
                    { 4, true, "Terminal B - Sur", "B2" },
                    { 5, true, "Terminal C - Internacional", "C1" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Slots_ScheduleTime_Runway",
                table: "Slots",
                columns: new[] { "ScheduleTime", "Runway" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boardings_TicketId_SlotId",
                table: "Boardings",
                columns: new[] { "TicketId", "SlotId" },
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
                name: "FK_Boardings_Slots_SlotId",
                table: "Boardings",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Boardings_Tickets_TicketId",
                table: "Boardings",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Gates_GateId",
                table: "Slots",
                column: "GateId",
                principalTable: "Gates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boardings_Slots_SlotId",
                table: "Boardings");

            migrationBuilder.DropForeignKey(
                name: "FK_Boardings_Tickets_TicketId",
                table: "Boardings");

            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Gates_GateId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_ScheduleTime_Runway",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Boardings_TicketId_SlotId",
                table: "Boardings");

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

            migrationBuilder.DeleteData(
                table: "Gates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Gates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Gates",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Gates",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Gates",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Seat",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PassengerName",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FlightCode",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

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
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "FlightCode",
                table: "Slots",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Gates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Gates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Airlines",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Airlines",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "ApiKey",
                table: "Airlines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Boardings_TicketId",
                table: "Boardings",
                column: "TicketId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Boardings_Slots_SlotId",
                table: "Boardings",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Boardings_Tickets_TicketId",
                table: "Boardings",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Gates_GateId",
                table: "Slots",
                column: "GateId",
                principalTable: "Gates",
                principalColumn: "Id");
        }
    }
}
