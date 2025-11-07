using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSlotReservationExpiry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReservationExpiresAt",
                table: "Slots",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservationExpiresAt",
                table: "Slots");
        }
    }
}
