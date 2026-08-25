using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class AuditLogMigrationSaveState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Json",
                table: "AuditLog",
                newName: "RecordStateBeforeChangeJson");

            migrationBuilder.AddColumn<string>(
                name: "ChangeLogJson",
                table: "AuditLog",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeLogJson",
                table: "AuditLog");

            migrationBuilder.RenameColumn(
                name: "RecordStateBeforeChangeJson",
                table: "AuditLog",
                newName: "Json");
        }
    }
}
