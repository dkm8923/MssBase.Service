using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserPasswordLengthUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Application",
                keyColumn: "ApplicationId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Application",
                keyColumn: "ApplicationId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Application",
                keyColumn: "ApplicationId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Application",
                keyColumn: "ApplicationId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ApplicationUser",
                keyColumn: "ApplicationUserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ApplicationUserPermission",
                keyColumn: "ApplicationUserPermissionId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "PermissionId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "PermissionId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "PermissionId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "PermissionId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumn: "RolePermissionId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Application",
                keyColumn: "ApplicationId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ApplicationUser",
                keyColumn: "ApplicationUserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "PermissionId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Application",
                keyColumn: "ApplicationId",
                keyValue: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "ApplicationUser",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldUnicode: false,
                oldMaxLength: 64,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "ApplicationUser",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Application",
                columns: new[] { "ApplicationId", "Active", "CreatedBy", "CreatedOn", "Description", "Name", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, true, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Enterprise Dispatch and Monitoring System for Logistic Operations", "EOS", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, true, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Enterprise Financial System for Processing Pricing & Commissions", "EPC", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, true, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Enterprise User Permission Management System", "EBS", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, true, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Interchange Configuration Tool", "Bet-t", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, true, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Agent Analytics / Reporting Portal", "MyPortfolio", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, true, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Agent Management Platform", "AIME", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "ApplicationUser",
                columns: new[] { "ApplicationUserId", "Active", "ApplicationId", "CreatedBy", "CreatedOn", "DateOfBirth", "Email", "FirstName", "LastLockoutDate", "LastLoginDate", "LastName", "LastPasswordChangeDate", "Password", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, true, 2, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "dkm8923@gmail.com", "Dan", null, null, "Mauk", null, null, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, true, 1, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "thompsonswartz@gmail.com", "Rachel", null, null, "Thompson", null, null, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "PermissionId", "Active", "ApplicationId", "CreatedBy", "CreatedOn", "Description", "Name", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, true, 1, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Default Base Permission for EOS Application", "EosDefaultUser", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, true, 2, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Default Base Permission for EPC Application", "EpcDefaultUser", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, true, 2, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Default Base Permission for Commission Reviewer UI / Services", "CommissionReviewer", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, true, 2, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permission for Allowing Access to Change Contractor on Commission Reviewer UI / Services", "CommissionReviewerChangeContractor", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, true, 2, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permission for Adjusting Contractor Rates on Commission Reviewer UI / Services", "CommissionReviewerAdjustRate", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleId", "Active", "ApplicationId", "CreatedBy", "CreatedOn", "Description", "Name", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, true, 2, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Super User Role for EPC Application", "DataAnalyst", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, true, 2, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Read Only Role for EPC Application", "OfficeUser", "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "ApplicationUserPermission",
                columns: new[] { "ApplicationUserPermissionId", "Active", "ApplicationId", "ApplicationUserId", "CreatedBy", "CreatedOn", "PermissionId", "UpdatedBy", "UpdatedOn" },
                values: new object[] { 1, true, 2, 1, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "RolePermissionId", "Active", "ApplicationId", "CreatedBy", "CreatedOn", "PermissionId", "RoleId", "UpdatedBy", "UpdatedOn" },
                values: new object[] { 1, true, 2, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "MssBase.Service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }
    }
}
