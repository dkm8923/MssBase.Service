using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class CreatedByUpdatedByMaxLengthUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ApplicationUser_Log_Login",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldUnicode: false,
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ApplicationUser_Log_ChangePassword",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldUnicode: false,
                oldMaxLength: 64);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ApplicationUser_Log_Login",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldUnicode: false,
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ApplicationUser_Log_ChangePassword",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldUnicode: false,
                oldMaxLength: 128);
        }
    }
}
