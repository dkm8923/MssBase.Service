using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Security.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    UpdatedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.ApplicationId);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    FirstName = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    LastName = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: true),
                    Password = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: true),
                    LastPasswordChangeDate = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: true),
                    LastLockoutDate = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: true),
                    FailedPasswordAttemptCount = table.Column<short>(type: "smallint", nullable: true, defaultValue: (short)0),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    UpdatedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.ApplicationUserId);
                    table.ForeignKey(
                        name: "FK_ApplicationUser_Application",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "ApplicationId");
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    UpdatedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.PermissionId);
                    table.ForeignKey(
                        name: "FK_Permission_Application",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "ApplicationId");
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    UpdatedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_Role_Application",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "ApplicationId");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserPermission",
                columns: table => new
                {
                    ApplicationUserPermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    UpdatedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserPermission", x => x.ApplicationUserPermissionId);
                    table.ForeignKey(
                        name: "FK_ApplicationUserPermission_Application",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "ApplicationId");
                    table.ForeignKey(
                        name: "FK_ApplicationUserPermission_Permission",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "PermissionId");
                    table.ForeignKey(
                        name: "FK_ApplicationUserPermission_User",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "ApplicationUserId");
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    RolePermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    UpdatedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.RolePermissionId);
                    table.ForeignKey(
                        name: "FK_RolePermission_Application",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "ApplicationId");
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "PermissionId");
                    table.ForeignKey(
                        name: "FK_RolePermission_Role",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId");
                });

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

            migrationBuilder.CreateIndex(
                name: "UQ_Application_Name",
                table: "Application",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_ApplicationId",
                table: "ApplicationUser",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationUser_Email",
                table: "ApplicationUser",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserPermission_ApplicationUserId",
                table: "ApplicationUserPermission",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserPermission_PermissionId",
                table: "ApplicationUserPermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationUserPermission_ApplicationId_ApplicationUserId_PermissionId",
                table: "ApplicationUserPermission",
                columns: new[] { "ApplicationId", "ApplicationUserId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permission_ApplicationId",
                table: "Permission",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "UQ_Permission_Name",
                table: "Permission",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_ApplicationId",
                table: "Role",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "UQ_Role_Name",
                table: "Role",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId",
                table: "RolePermission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQ_RolePermission_ApplicationId_RoleId_PermissionId",
                table: "RolePermission",
                columns: new[] { "ApplicationId", "RoleId", "PermissionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserPermission");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "ApplicationUser");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Application");
        }
    }
}
