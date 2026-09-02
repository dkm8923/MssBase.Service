using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class RemoveApplicationUserFunctionalityMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUser_Log_ChangePassword");

            migrationBuilder.DropTable(
                name: "ApplicationUser_Log_Login");

            migrationBuilder.DropTable(
                name: "ApplicationUserLogin");

            migrationBuilder.DropIndex(
                name: "UQ_ApplicationUser_Email",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "ApplicationUser");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ApplicationUser",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationUser_UserId_ApplicationId",
                table: "ApplicationUser",
                columns: new[] { "UserId", "ApplicationId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_User",
                table: "ApplicationUser",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUser_User",
                table: "ApplicationUser");

            migrationBuilder.DropIndex(
                name: "UQ_ApplicationUser_UserId_ApplicationId",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ApplicationUser");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "ApplicationUser",
                type: "datetime2(2)",
                precision: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ApplicationUser",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "ApplicationUser",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "ApplicationUser",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUser_Log_ChangePassword",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    OldPassword = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser_Log_ChangePassword", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_ApplicationUser_Log_ChangePassword_Application",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "ApplicationId");
                    table.ForeignKey(
                        name: "FK_ApplicationUser_Log_ChangePassword_User",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "ApplicationUserId");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUser_Log_Login",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser_Log_Login", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_ApplicationUser_Log_Login_Application",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "ApplicationId");
                    table.ForeignKey(
                        name: "FK_ApplicationUser_Log_Login_User",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "ApplicationUserId");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserLogin",
                columns: table => new
                {
                    ApplicationUserLoginId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false),
                    FailedPasswordAttemptCount = table.Column<short>(type: "smallint", nullable: true, defaultValue: (short)0),
                    LastLockoutDate = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: true),
                    LastPasswordChangeDate = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PasswordResetRequired = table.Column<bool>(type: "bit", nullable: false),
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
                name: "UQ_ApplicationUser_Email",
                table: "ApplicationUser",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_Log_ChangePassword_ApplicationId",
                table: "ApplicationUser_Log_ChangePassword",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationUser_Log_ChangePassword_ApplicationUserId_ApplicationId_OldPassword",
                table: "ApplicationUser_Log_ChangePassword",
                columns: new[] { "ApplicationUserId", "ApplicationId", "OldPassword" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_Log_Login_ApplicationId",
                table: "ApplicationUser_Log_Login",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationUser_Log_Login_ApplicationUserId_ApplicationId_CreatedOn",
                table: "ApplicationUser_Log_Login",
                columns: new[] { "ApplicationUserId", "ApplicationId", "CreatedOn" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserLogin_ApplicationId",
                table: "ApplicationUserLogin",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserLogin_ApplicationUserId",
                table: "ApplicationUserLogin",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationUserLogin_ApplicationUserId_ApplicationId",
                table: "ApplicationUserLogin",
                columns: new[] { "ApplicationUserId", "ApplicationId" },
                unique: true);
        }
    }
}
