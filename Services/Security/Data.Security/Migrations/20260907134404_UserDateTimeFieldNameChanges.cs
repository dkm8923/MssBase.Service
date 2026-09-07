using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class UserDateTimeFieldNameChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastPasswordChangeDate",
                table: "UserLogin",
                newName: "LastPasswordChangeDateTime");

            migrationBuilder.RenameColumn(
                name: "LastLoginDate",
                table: "UserLogin",
                newName: "LastLoginDateTime");

            migrationBuilder.RenameColumn(
                name: "LastLockoutDate",
                table: "UserLogin",
                newName: "LastLockoutDateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastPasswordChangeDateTime",
                table: "UserLogin",
                newName: "LastPasswordChangeDate");

            migrationBuilder.RenameColumn(
                name: "LastLoginDateTime",
                table: "UserLogin",
                newName: "LastLoginDate");

            migrationBuilder.RenameColumn(
                name: "LastLockoutDateTime",
                table: "UserLogin",
                newName: "LastLockoutDate");
        }
    }
}
