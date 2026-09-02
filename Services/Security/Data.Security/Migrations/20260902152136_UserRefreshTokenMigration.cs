using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class UserRefreshTokenMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "UserLogin");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "UserLogin");

            migrationBuilder.CreateTable(
                name: "UserRefreshToken",
                columns: table => new
                {
                    UserRefreshTokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    RefreshToken = table.Column<string>(type: "varchar(2048)", unicode: false, maxLength: 2048, nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRefreshToken", x => x.UserRefreshTokenId);
                    table.ForeignKey(
                        name: "FK_UserRefreshToken_Application",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "ApplicationId");
                    table.ForeignKey(
                        name: "FK_UserRefreshToken_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshToken_ApplicationId",
                table: "UserRefreshToken",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "UQ_UserRefreshToken_UserId_ApplicationId",
                table: "UserRefreshToken",
                columns: new[] { "UserId", "ApplicationId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "UserLogin",
                type: "varchar(2048)",
                unicode: false,
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "UserLogin",
                type: "datetime2(2)",
                precision: 2,
                nullable: true);
        }
    }
}
