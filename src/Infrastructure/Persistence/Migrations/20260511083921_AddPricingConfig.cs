using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PricingConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClubId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RainDiscountPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    WindThreshold = table.Column<double>(type: "double", nullable: false),
                    WindDiscountPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    HeatThreshold = table.Column<double>(type: "double", nullable: false),
                    HeatDiscountPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ColdThreshold = table.Column<double>(type: "double", nullable: false),
                    ColdDiscountPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricingConfigs_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7c9e66ab-7839-47e2-9383-718693c04200"),
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 8, 39, 21, 637, DateTimeKind.Utc).AddTicks(7260));

            migrationBuilder.CreateIndex(
                name: "IX_PricingConfigs_ClubId",
                table: "PricingConfigs",
                column: "ClubId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricingConfigs");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7c9e66ab-7839-47e2-9383-718693c04200"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 26, 14, 23, 33, 268, DateTimeKind.Utc).AddTicks(4030));
        }
    }
}
