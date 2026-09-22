using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUsernameToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                schema: "identity",
                table: "users",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            // Dev-data backfill: derive a username from the email local-part for any
            // pre-existing rows (this project has no production data yet).
            migrationBuilder.Sql(
                """
                UPDATE identity.users
                SET "Username" = split_part("Email", '@', 1)
                WHERE "Username" = '';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                schema: "identity",
                table: "users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_Username",
                schema: "identity",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Username",
                schema: "identity",
                table: "users");
        }
    }
}
