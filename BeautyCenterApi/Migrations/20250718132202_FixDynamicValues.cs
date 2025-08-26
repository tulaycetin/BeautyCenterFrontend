using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeautyCenterApi.Migrations
{
    /// <inheritdoc />
    public partial class FixDynamicValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 18, 13, 22, 2, 178, DateTimeKind.Utc).AddTicks(1661));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 18, 13, 22, 2, 178, DateTimeKind.Utc).AddTicks(1881));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 18, 13, 22, 2, 178, DateTimeKind.Utc).AddTicks(1883));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 18, 13, 22, 2, 178, DateTimeKind.Utc).AddTicks(1884));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 18, 13, 22, 2, 178, DateTimeKind.Utc).AddTicks(1886));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 7, 18, 13, 22, 2, 177, DateTimeKind.Utc).AddTicks(2650), "$2a$11$WHz3BKA8RnoMEsFqgOMNpuTiyXYsmM.3SaL1g0OjC/0HYUSekeLb." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 18, 13, 7, 10, 300, DateTimeKind.Utc).AddTicks(6226));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 18, 13, 7, 10, 300, DateTimeKind.Utc).AddTicks(6432));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 18, 13, 7, 10, 300, DateTimeKind.Utc).AddTicks(6433));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 18, 13, 7, 10, 300, DateTimeKind.Utc).AddTicks(6435));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 18, 13, 7, 10, 300, DateTimeKind.Utc).AddTicks(6436));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 7, 18, 13, 7, 10, 299, DateTimeKind.Utc).AddTicks(8436), "$2a$11$VIUmlAWIaoaPik9ny0ugxO7K5g3i0i9cFBepNnlyZGU7Dxh31dVYG" });
        }
    }
}
