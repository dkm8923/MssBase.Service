using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserLoginMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUserLogin",
                columns: table => new
                {
                    ApplicationUserLoginId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PasswordResetRequired = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: true),
                    LastPasswordChangeDate = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: true),
                    LastLockoutDate = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: true),
                    FailedPasswordAttemptCount = table.Column<short>(type: "smallint", nullable: true, defaultValue: (short)0),
                    RefreshToken = table.Column<string>(type: "varchar(2048)", unicode: false, maxLength: 2048, nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserLogin", x => x.ApplicationUserLoginId);
                    table.ForeignKey(
                        name: "FK_ApplicationUserLogin_Application",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "ApplicationId");
                    table.ForeignKey(
                        name: "FK_ApplicationUserLogin_ApplicationUser",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "ApplicationUserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserLogin_ApplicationId",
                table: "ApplicationUserLogin",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationUserLogin_ApplicationUserId_ApplicationId",
                table: "ApplicationUserLogin",
                columns: new[] { "ApplicationUserId", "ApplicationId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserLogin");
        }
    }
}
