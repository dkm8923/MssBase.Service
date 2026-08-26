using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserRemoveColumnsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedPasswordAttemptCount",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "LastLockoutDate",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "LastPasswordChangeDate",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "PasswordResetRequired",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "ApplicationUser");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId1",
                table: "ApplicationUserLogin",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserLogin_ApplicationUserId1",
                table: "ApplicationUserLogin",
                column: "ApplicationUserId1",
                unique: true,
                filter: "[ApplicationUserId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserLogin_ApplicationUser_ApplicationUserId1",
                table: "ApplicationUserLogin",
                column: "ApplicationUserId1",
                principalTable: "ApplicationUser",
                principalColumn: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserLogin_ApplicationUser_ApplicationUserId1",
                table: "ApplicationUserLogin");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserLogin_ApplicationUserId1",
                table: "ApplicationUserLogin");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "ApplicationUserLogin");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "ApplicationUser",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<short>(
                name: "FailedPasswordAttemptCount",
                table: "ApplicationUser",
                type: "smallint",
                nullable: true,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLockoutDate",
                table: "ApplicationUser",
                type: "datetime2(2)",
                precision: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "ApplicationUser",
                type: "datetime2(2)",
                precision: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPasswordChangeDate",
                table: "ApplicationUser",
                type: "datetime2(2)",
                precision: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PasswordResetRequired",
                table: "ApplicationUser",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "ApplicationUser",
                type: "varchar(2048)",
                unicode: false,
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "ApplicationUser",
                type: "datetime2(2)",
                precision: 2,
                nullable: true);
        }
    }
}
