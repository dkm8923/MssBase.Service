using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class UserTableInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    FirstName = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    LastName = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    ReadOnly = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    UpdatedBy = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                columns: table => new
                {
                    UserLoginId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_UserLogin", x => x.UserLoginId);
                    table.ForeignKey(
                        name: "FK_UserLogin_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "UQ_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_UserLogin_UserId",
                table: "UserLogin",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
