using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class _3_14_26_ApplicationUserRole_Take2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserRoles_ApplicationUser_ApplicationUserId",
                table: "ApplicationUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserRoles_Application_ApplicationId",
                table: "ApplicationUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserRoles_Role_RoleId",
                table: "ApplicationUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserRoles",
                table: "ApplicationUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserRoles_ApplicationId",
                table: "ApplicationUserRoles");

            migrationBuilder.RenameTable(
                name: "ApplicationUserRoles",
                newName: "ApplicationUserRole");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserRoles_RoleId",
                table: "ApplicationUserRole",
                newName: "IX_ApplicationUserRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserRoles_ApplicationUserId",
                table: "ApplicationUserRole",
                newName: "IX_ApplicationUserRole_ApplicationUserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedOn",
                table: "ApplicationUserRole",
                type: "datetime2(2)",
                precision: 2,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "ApplicationUserRole",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "ApplicationUserRole",
                type: "datetime2(2)",
                precision: 2,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ApplicationUserRole",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserRole",
                table: "ApplicationUserRole",
                column: "ApplicationUserRoleId");

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationUserRole_ApplicationId_ApplicationUserId_RoleId",
                table: "ApplicationUserRole",
                columns: new[] { "ApplicationId", "ApplicationUserId", "RoleId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserRole_Application",
                table: "ApplicationUserRole",
                column: "ApplicationId",
                principalTable: "Application",
                principalColumn: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserRole_Role",
                table: "ApplicationUserRole",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserRole_User",
                table: "ApplicationUserRole",
                column: "ApplicationUserId",
                principalTable: "ApplicationUser",
                principalColumn: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserRole_Application",
                table: "ApplicationUserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserRole_Role",
                table: "ApplicationUserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserRole_User",
                table: "ApplicationUserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserRole",
                table: "ApplicationUserRole");

            migrationBuilder.DropIndex(
                name: "UQ_ApplicationUserRole_ApplicationId_ApplicationUserId_RoleId",
                table: "ApplicationUserRole");

            migrationBuilder.RenameTable(
                name: "ApplicationUserRole",
                newName: "ApplicationUserRoles");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserRole_RoleId",
                table: "ApplicationUserRoles",
                newName: "IX_ApplicationUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserRole_ApplicationUserId",
                table: "ApplicationUserRoles",
                newName: "IX_ApplicationUserRoles_ApplicationUserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedOn",
                table: "ApplicationUserRoles",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(2)",
                oldPrecision: 2);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "ApplicationUserRoles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldUnicode: false,
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "ApplicationUserRoles",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(2)",
                oldPrecision: 2);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ApplicationUserRoles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldUnicode: false,
                oldMaxLength: 64);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserRoles",
                table: "ApplicationUserRoles",
                column: "ApplicationUserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserRoles_ApplicationId",
                table: "ApplicationUserRoles",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserRoles_ApplicationUser_ApplicationUserId",
                table: "ApplicationUserRoles",
                column: "ApplicationUserId",
                principalTable: "ApplicationUser",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserRoles_Application_ApplicationId",
                table: "ApplicationUserRoles",
                column: "ApplicationId",
                principalTable: "Application",
                principalColumn: "ApplicationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserRoles_Role_RoleId",
                table: "ApplicationUserRoles",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
