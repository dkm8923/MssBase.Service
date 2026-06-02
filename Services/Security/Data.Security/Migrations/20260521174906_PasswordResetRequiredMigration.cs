using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class PasswordResetRequiredMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PasswordResetRequired",
                table: "ApplicationUser",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetRequired",
                table: "ApplicationUser");
        }
    }
}
