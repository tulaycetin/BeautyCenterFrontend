using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeautyCenterApi.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$dm4AgGac/is.r.qEOtFP5.xAOUJCqrfVWJXzbe9O53OfpJvb0dEmK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$8VqSVRJMKqUHvuUHbdOhA.yZoqOHZxKQgGRhSKvKWJkfYbcWgCiYO");
        }
    }
}
