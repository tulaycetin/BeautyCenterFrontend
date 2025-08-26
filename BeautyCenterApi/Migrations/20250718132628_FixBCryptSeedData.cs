using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeautyCenterApi.Migrations
{
    /// <inheritdoc />
    public partial class FixBCryptSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$8VqSVRJMKqUHvuUHbdOhA.yZoqOHZxKQgGRhSKvKWJkfYbcWgCiYO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$X0lmQQeZGbzliEG1D3IFx.CBRTVurWfxM1I2omwSO9IJ6mHRxBQ.i");
        }
    }
}
