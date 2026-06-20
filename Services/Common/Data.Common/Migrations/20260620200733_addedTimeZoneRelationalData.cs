using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Common.Migrations
{
    /// <inheritdoc />
    public partial class addedTimeZoneRelationalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CommonRelationalData",
                columns: new[] { "CommonRelationalDataId", "Active", "CreatedBy", "CreatedOn", "Description", "Json", "ReferenceType", "UpdatedBy", "UpdatedOn" },
                values: new object[] { 12, true, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "List of all USA Time Zones and their Value", "[{\"Name\":\"Eastern Standard Time (EST):\",\"Value\":\"EST\",\"Description\":\"Covers the East Coast and parts of the Midwest\",\"Active\":true,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Central Standard Time (CST):\",\"Value\":\"CST\",\"Description\":\"Spans the central US and Gulf Coast\",\"Active\":true,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Mountain Standard Time (MST):\",\"Value\":\"MST\",\"Description\":\"Extends across the Mountain West\",\"Active\":true,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Pacific Standard Time (PST):\",\"Value\":\"PST\",\"Description\":\"Covers the West Coast, including California, and Nevada\",\"Active\":true,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Alaska Standard Time (AKST):\",\"Value\":\"AKST\",\"Description\":\"Covers almost the entire state of Alaska\",\"Active\":true,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Hawaii-Aleutian Standard Time (HST)\",\"Value\":\"HST\",\"Description\":\"Covers Hawaii and parts of the Aleutian Islands in Alaska\",\"Active\":true,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Atlantic Standard Time (AST)\",\"Value\":\"AST\",\"Description\":\"Puerto Rico and the US Virgin Islands\",\"Active\":true,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Samoa Standard Time (SST)\",\"Value\":\"SST\",\"Description\":\"American Samoa\",\"Active\":true,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Chamorro Standard Time (CHST)\",\"Value\":\"CHST\",\"Description\":\"Guam and the Northern Mariana Islands\",\"Active\":true,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"}]", "UsaTimeZone", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CommonRelationalData",
                keyColumn: "CommonRelationalDataId",
                keyValue: 12);
        }
    }
}
