using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Common.Migrations
{
    /// <inheritdoc />
    public partial class DaysOfWeekRelationalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CommonRelationalData",
                columns: new[] { "CommonRelationalDataId", "Active", "CreatedBy", "CreatedOn", "Description", "Json", "ReadOnly", "ReferenceType", "UpdatedBy", "UpdatedOn" },
                values: new object[] { 17, true, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "List of all Days of the Week and their Value", "[{\"Name\":\"Monday\",\"Value\":\"MON\",\"SortOrder\":1,\"Active\":true,\"ReadOnly\":false,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Tuesday\",\"Value\":\"TUES\",\"SortOrder\":2,\"Active\":true,\"ReadOnly\":false,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Wednesday\",\"Value\":\"WED\",\"SortOrder\":3,\"Active\":true,\"ReadOnly\":false,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Thursday\",\"Value\":\"THURS\",\"SortOrder\":4,\"Active\":true,\"ReadOnly\":false,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Friday\",\"Value\":\"FRI\",\"SortOrder\":5,\"Active\":true,\"ReadOnly\":false,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Saturday\",\"Value\":\"SAT\",\"SortOrder\":6,\"Active\":true,\"ReadOnly\":false,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Sunday\",\"Value\":\"SUN\",\"SortOrder\":7,\"Active\":true,\"ReadOnly\":false,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"}]", false, "DaysOfWeek", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CommonRelationalData",
                keyColumn: "CommonRelationalDataId",
                keyValue: 17);
        }
    }
}
