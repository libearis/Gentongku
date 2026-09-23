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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    buyer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders_indexed", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "orders_plain",
                schema: "benchmark",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    buyer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders_plain", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_orders_indexed_buyer_id",
                schema: "benchmark",
                table: "orders_indexed",
                column: "buyer_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_indexed_created_at",
                schema: "benchmark",
                table: "orders_indexed",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_orders_indexed_status",
                schema: "benchmark",
                table: "orders_indexed",
                column: "status");
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
