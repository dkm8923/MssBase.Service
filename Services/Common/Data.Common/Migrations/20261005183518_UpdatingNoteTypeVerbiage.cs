using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Common.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingNoteTypeVerbiage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CommonRelationalData",
                keyColumn: "CommonRelationalDataId",
                keyValue: 18,
                columns: new[] { "Description", "ReferenceType" },
                values: new object[] { "List of all Note Types and their Value", "NoteType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CommonRelationalData",
                keyColumn: "CommonRelationalDataId",
                keyValue: 18,
                columns: new[] { "Description", "ReferenceType" },
                values: new object[] { "List of all Common Note Types and their Value", "CommonNoteType" });
        }
    }
}
