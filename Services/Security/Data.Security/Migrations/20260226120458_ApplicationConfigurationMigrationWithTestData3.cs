using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationConfigurationMigrationWithTestData3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Application",
                keyColumn: "ApplicationId",
                keyValue: 1,
                columns: new[] { "CreatedOn", "UpdatedOn" },
                values: new object[] { new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Application",
                keyColumn: "ApplicationId",
                keyValue: 2,
                columns: new[] { "CreatedOn", "UpdatedOn" },
                values: new object[] { new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Application",
                keyColumn: "ApplicationId",
                keyValue: 1,
                columns: new[] { "CreatedOn", "UpdatedOn" },
                values: new object[] { new DateTime(2026, 2, 26, 11, 56, 9, 505, DateTimeKind.Utc).AddTicks(1889), new DateTime(2026, 2, 26, 11, 56, 9, 505, DateTimeKind.Utc).AddTicks(2539) });

            migrationBuilder.UpdateData(
                table: "Application",
                keyColumn: "ApplicationId",
                keyValue: 2,
                columns: new[] { "CreatedOn", "UpdatedOn" },
                values: new object[] { new DateTime(2026, 2, 26, 11, 56, 9, 505, DateTimeKind.Utc).AddTicks(3541), new DateTime(2026, 2, 26, 11, 56, 9, 505, DateTimeKind.Utc).AddTicks(3542) });
        }
    }
}
