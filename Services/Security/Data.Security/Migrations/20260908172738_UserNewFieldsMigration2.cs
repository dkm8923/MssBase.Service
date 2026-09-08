using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class UserNewFieldsMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlternateEmailJson",
                table: "User",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "User",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "User",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumberJson",
                table: "User",
                type: "varchar(1024)",
                unicode: false,
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Religion",
                table: "User",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sexuality",
                table: "User",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialMediaProfileJson",
                table: "User",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpokenLanguagesJson",
                table: "User",
                type: "varchar(512)",
                unicode: false,
                maxLength: 512,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternateEmailJson",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "User");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PhoneNumberJson",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Religion",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Sexuality",
                table: "User");

            migrationBuilder.DropColumn(
                name: "SocialMediaProfileJson",
                table: "User");

            migrationBuilder.DropColumn(
                name: "SpokenLanguagesJson",
                table: "User");
        }
    }
}
