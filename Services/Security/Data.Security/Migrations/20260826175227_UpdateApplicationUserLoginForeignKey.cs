using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApplicationUserLoginForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserLogin_ApplicationUserId",
                table: "ApplicationUserLogin",
                column: "ApplicationUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserLogin_ApplicationUserId",
                table: "ApplicationUserLogin");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId1",
                table: "ApplicationUserLogin",
                type: "int",
                nullable: true);

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
    }
}
