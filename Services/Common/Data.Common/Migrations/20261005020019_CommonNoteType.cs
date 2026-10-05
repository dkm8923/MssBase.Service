using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Common.Migrations
{
    /// <inheritdoc />
    public partial class CommonNoteType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CommonRelationalData",
                columns: new[] { "CommonRelationalDataId", "Active", "CreatedBy", "CreatedOn", "Description", "Json", "ReadOnly", "ReferenceType", "UpdatedBy", "UpdatedOn" },
                values: new object[] { 18, true, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "List of all Common Note Types and their Value", "[{\"Name\":\"Information\",\"Value\":\"Information\",\"SortOrder\":1,\"Active\":true,\"ReadOnly\":false,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Warning\",\"Value\":\"Warning\",\"SortOrder\":2,\"Active\":true,\"ReadOnly\":false,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Danger\",\"Value\":\"Danger\",\"SortOrder\":3,\"Active\":true,\"ReadOnly\":false,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"},{\"Name\":\"Success\",\"Value\":\"Success\",\"SortOrder\":4,\"Active\":true,\"ReadOnly\":false,\"CreatedOn\":\"2026-01-01T00:00:00\",\"CreatedBy\":\"MssBase.Service\",\"UpdatedOn\":\"2026-01-01T00:00:00\",\"UpdatedBy\":\"MssBase.Service\"}]", false, "CommonNoteType", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CommonRelationalData",
                keyColumn: "CommonRelationalDataId",
                keyValue: 18);
        }
    }
}
