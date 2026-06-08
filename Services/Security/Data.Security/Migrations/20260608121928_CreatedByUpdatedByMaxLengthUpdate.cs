using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class CreatedByUpdatedByMaxLengthUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "RolePermission",
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
                table: "RolePermission",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldUnicode: false,
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Role",
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
                table: "Role",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldUnicode: false,
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Permission",
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
                table: "Permission",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldUnicode: false,
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "ApplicationUserRole",
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
                table: "ApplicationUserRole",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldUnicode: false,
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "ApplicationUserPermission",
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
                table: "ApplicationUserPermission",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldUnicode: false,
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "ApplicationUser",
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
                table: "ApplicationUser",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldUnicode: false,
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Application",
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
                table: "Application",
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
                name: "UpdatedBy",
                table: "RolePermission",
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
                table: "RolePermission",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldUnicode: false,
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Role",
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
                table: "Role",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldUnicode: false,
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Permission",
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
                table: "Permission",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldUnicode: false,
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "ApplicationUserRole",
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
                table: "ApplicationUserRole",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldUnicode: false,
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "ApplicationUserPermission",
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
                table: "ApplicationUserPermission",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldUnicode: false,
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "ApplicationUser",
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
                table: "ApplicationUser",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldUnicode: false,
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Application",
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
                table: "Application",
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
