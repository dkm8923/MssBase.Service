using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationConfigurationMigrationWithTestData2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Application",
                columns: new[] { "ApplicationId", "Active", "CreatedBy", "CreatedOn", "Description", "Name", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, true, "dmaukTest", new DateTime(2026, 2, 26, 11, 56, 9, 505, DateTimeKind.Utc).AddTicks(1889), "Description1", "Application1", "dmaukTest", new DateTime(2026, 2, 26, 11, 56, 9, 505, DateTimeKind.Utc).AddTicks(2539) },
                    { 2, true, "dmaukTest", new DateTime(2026, 2, 26, 11, 56, 9, 505, DateTimeKind.Utc).AddTicks(3541), "Description2", "Application2", "dmaukTest", new DateTime(2026, 2, 26, 11, 56, 9, 505, DateTimeKind.Utc).AddTicks(3542) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Application",
                keyColumn: "ApplicationId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Application",
                keyColumn: "ApplicationId",
                keyValue: 2);
        }
    }
}
