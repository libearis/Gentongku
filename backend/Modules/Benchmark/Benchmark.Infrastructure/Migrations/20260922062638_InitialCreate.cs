using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Benchmark.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "benchmark");

            migrationBuilder.CreateTable(
                name: "orders_indexed",
                schema: "benchmark",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders_indexed", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orders_plain",
                schema: "benchmark",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders_plain", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_indexed_BuyerId",
                schema: "benchmark",
                table: "orders_indexed",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_indexed_CreatedAt",
                schema: "benchmark",
                table: "orders_indexed",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_orders_indexed_Status",
                schema: "benchmark",
                table: "orders_indexed",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orders_indexed",
                schema: "benchmark");

            migrationBuilder.DropTable(
                name: "orders_plain",
                schema: "benchmark");
        }
    }
}
