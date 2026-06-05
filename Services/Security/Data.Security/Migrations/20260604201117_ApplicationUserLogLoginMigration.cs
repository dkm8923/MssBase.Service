using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserLogLoginMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserLogChangePassword_Application",
                table: "ApplicationUserLogChangePassword");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserLogChangePassword_User",
                table: "ApplicationUserLogChangePassword");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserLogChangePassword",
                table: "ApplicationUserLogChangePassword");

            migrationBuilder.RenameTable(
                name: "ApplicationUserLogChangePassword",
                newName: "ApplicationUser_Log_ChangePassword");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserLogChangePasswordId",
                table: "ApplicationUser_Log_ChangePassword",
                newName: "LogId");

            migrationBuilder.RenameIndex(
                name: "UQ_ApplicationUserLogChangePassword_ApplicationUserId_ApplicationId_OldPassword",
                table: "ApplicationUser_Log_ChangePassword",
                newName: "UQ_ApplicationUser_Log_ChangePassword_ApplicationUserId_ApplicationId_OldPassword");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserLogChangePassword_ApplicationId",
                table: "ApplicationUser_Log_ChangePassword",
                newName: "IX_ApplicationUser_Log_ChangePassword_ApplicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUser_Log_ChangePassword",
                table: "ApplicationUser_Log_ChangePassword",
                column: "LogId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_Log_ChangePassword_Application",
                table: "ApplicationUser_Log_ChangePassword",
                column: "ApplicationId",
                principalTable: "Application",
                principalColumn: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_Log_ChangePassword_User",
                table: "ApplicationUser_Log_ChangePassword",
                column: "ApplicationUserId",
                principalTable: "ApplicationUser",
                principalColumn: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUser_Log_ChangePassword_Application",
                table: "ApplicationUser_Log_ChangePassword");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUser_Log_ChangePassword_User",
                table: "ApplicationUser_Log_ChangePassword");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUser_Log_ChangePassword",
                table: "ApplicationUser_Log_ChangePassword");

            migrationBuilder.RenameTable(
                name: "ApplicationUser_Log_ChangePassword",
                newName: "ApplicationUserLogChangePassword");

            migrationBuilder.RenameColumn(
                name: "LogId",
                table: "ApplicationUserLogChangePassword",
                newName: "ApplicationUserLogChangePasswordId");

            migrationBuilder.RenameIndex(
                name: "UQ_ApplicationUser_Log_ChangePassword_ApplicationUserId_ApplicationId_OldPassword",
                table: "ApplicationUserLogChangePassword",
                newName: "UQ_ApplicationUserLogChangePassword_ApplicationUserId_ApplicationId_OldPassword");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUser_Log_ChangePassword_ApplicationId",
                table: "ApplicationUserLogChangePassword",
                newName: "IX_ApplicationUserLogChangePassword_ApplicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserLogChangePassword",
                table: "ApplicationUserLogChangePassword",
                column: "ApplicationUserLogChangePasswordId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserLogChangePassword_Application",
                table: "ApplicationUserLogChangePassword",
                column: "ApplicationId",
                principalTable: "Application",
                principalColumn: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserLogChangePassword_User",
                table: "ApplicationUserLogChangePassword",
                column: "ApplicationUserId",
                principalTable: "ApplicationUser",
                principalColumn: "ApplicationUserId");
        }
    }
}
