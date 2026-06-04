using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class LogChangePasswordMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUserLogChangePassword",
                columns: table => new
                {
                    ApplicationUserLogChangePasswordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    OldPassword = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserLogChangePassword", x => x.ApplicationUserLogChangePasswordId);
                    table.ForeignKey(
                        name: "FK_ApplicationUserLogChangePassword_Application",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "ApplicationId");
                    table.ForeignKey(
                        name: "FK_ApplicationUserLogChangePassword_User",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "ApplicationUserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserLogChangePassword_ApplicationId",
                table: "ApplicationUserLogChangePassword",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationUserLogChangePassword_ApplicationUserId_ApplicationId_OldPassword",
                table: "ApplicationUserLogChangePassword",
                columns: new[] { "ApplicationUserId", "ApplicationId", "OldPassword" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserLogChangePassword");
        }
    }
}
