using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentitySeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("7c9e66ab-7839-47e2-9383-718693c04200"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4c96213b-552a-4f71-a484-297698731dda", "AQAAAAIAAYagAAAAEGcBTmpi2fs/ZEkgeoinrn2dn+kIoVeyGnW03IehsGCXUnHfGaMhjGfP/bwHsuJyEA==", "eed283a0-4f90-4e7f-9e29-53070cbe0ba5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("7c9e66ab-7839-47e2-9383-718693c04200"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9cd76270-0080-489d-b611-a31fc92137f3", "AQAAAAIAAYagAAAAEAZesPT7peC7HEQ3nSev41zysFV3uWKUuo0FEQkzu6SC4IvxedLMoDqogPQrPr6vFg==", "cf4bb0a3-b9d3-4967-8023-0a94b4d5250a" });
        }
    }
}
