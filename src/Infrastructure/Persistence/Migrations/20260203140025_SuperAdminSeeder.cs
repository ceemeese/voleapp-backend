using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SuperAdminSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Dni", "Email", "IsActive", "LastName", "Name", "PhoneNumber", "Username" },
                values: new object[] { new Guid("7c9e66ab-7839-47e2-9383-718693c04200"), new DateTime(2026, 2, 3, 14, 0, 25, 49, DateTimeKind.Utc).AddTicks(9940), "00000000A", "superadminadmin@voleapp.es", true, "Superadmin", "SuperAdmin", "000000000", "superadmin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7c9e66ab-7839-47e2-9383-718693c04200"));
        }
    }
}
