using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class AuditLogMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditChangeLog");

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    AuditLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: false),
                    Json = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.AuditLogId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.CreateTable(
                name: "AuditChangeLog",
                columns: table => new
                {
                    AuditChangeLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChangeType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    Json = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditChangeLog", x => x.AuditChangeLogId);
                });
        }
    }
}
