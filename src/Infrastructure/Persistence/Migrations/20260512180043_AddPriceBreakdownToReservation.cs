using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceBreakdownToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Reservations",
                newName: "Price_TotalPrice");

            migrationBuilder.AddColumn<double>(
                name: "Price_AppliedDiscountPercent",
                table: "Reservations",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price_BasePrice",
                table: "Reservations",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price_DiscountAmount",
                table: "Reservations",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Price_DiscountReason",
                table: "Reservations",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7c9e66ab-7839-47e2-9383-718693c04200"),
                column: "CreatedAt",
                value: new DateTime(2026, 5, 12, 18, 0, 43, 694, DateTimeKind.Utc).AddTicks(8580));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price_AppliedDiscountPercent",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Price_BasePrice",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Price_DiscountAmount",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Price_DiscountReason",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "Price_TotalPrice",
                table: "Reservations",
                newName: "TotalPrice");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7c9e66ab-7839-47e2-9383-718693c04200"),
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 8, 39, 21, 637, DateTimeKind.Utc).AddTicks(7260));
        }
    }
}
