using Dto.Security.Application;
using Dto.Security.Application.Service;
using FluentAssertions;
using IntegrationTests.Security.Shared;
using IntegrationTests.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using Shared.Models;
using System.Net;
using IntegrationTests.Shared.Utilities;
using Dto.Security.ApplicationUser;
using Dto.Security.Permission;
using Dto.Security.Role;
using Dto.Security.ApplicationUserPermission;
using Dto.Security.RolePermission;
using Dto.Security.ApplicationUserRole;

namespace IntegrationTests.Security.Shared.SeedDatabase;

[Collection("SecurityIntegrationTests")]
public class SeedSecurityDB : SecurityTestBase, IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SeedSecurityDB(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region utils

    #endregion

    // [Fact]
    // public async Task Seed_SecurityDB()
    // {
    //     // Create test data for manual testing purposes
    //     await ClearAllSecurityTestTableData();

    //     await DropDatabaseAndRecreate();

    //     var applications = await CreateTestApplications();
        
    //     var applicationUsers = await CreateTestApplicationUsers(applications);
    //     var permissions = await CreateTestPermissions(applications);
    //     var applicationUserPermissions = await CreateTestApplicationUserPermissions(applications, applicationUsers, permissions);
    //     var roles = await CreateTestRoles(applications);

    //     await CreateSpecificTestData();

    //     // Assert
    //     1.Should().Be(1);
    // }

    [Fact]
    public async Task Seed_SecurityDB()
    {
        // Create test data for manual testing purposes
        await ClearAllSecurityTestTableData();

        await DropDatabaseAndRecreate();

        // var applications = await CreateTestApplications();
        
        // var applicationUsers = await CreateTestApplicationUsers(applications);
        // var permissions = await CreateTestPermissions(applications);
        // var applicationUserPermissions = await CreateTestApplicationUserPermissions(applications, applicationUsers, permissions);
        // var roles = await CreateTestRoles(applications);

        await CreateSpecificTestData();

        // Assert
        1.Should().Be(1);
    }

    private async Task CreateSpecificTestData()
    {
        //Create Application
        //var applicationReq = new InsertUpdateApplicationRequest { CurrentUser = TestConstants.CurrentUser, Active = true, Name = "Workout Tracker App", Description = "Keeps Track of Workout Sets / Reps" };    
        var applicationReq = new InsertUpdateApplicationRequest { CurrentUser = TestConstants.CurrentUser, Active = true, Name = "Test Application 1", Description = "Test Application 1 Description" };    
        var insertedApp = await _applicationLogic.Insert(applicationReq);
        var applicationId = insertedApp.Response.ApplicationId;
        
        //Create Application usrs
        var applicationUsersToCreate = new List<InsertUpdateApplicationUserRequest>();
        applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)applicationId, Email = "dmauk@echohealthinc.com", FirstName = "Daniel", LastName = "Mauk", DateOfBirth = new DateTime(1989, 6, 15) });
        applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)applicationId, Email = "rthompson@metrohealth.org", FirstName = "Rachel", LastName = "Thompson", DateOfBirth = new DateTime(1987, 12, 04) });
        applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)applicationId, Email = "PawPatrolOverEverything@gmail.com", FirstName = "Laura", LastName = "Mauk", DateOfBirth = new DateTime(2019, 9, 2) });
        applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)applicationId, Email = "BigTruckPup@yahoo.com", FirstName = "Cameron", LastName = "Mauk", DateOfBirth = new DateTime(2022, 5, 19) });
        applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)applicationId, Email = "SweetWilliam@aol.com", FirstName = "William", LastName = "Mauk", DateOfBirth = new DateTime(2024, 12, 15) });
        
        var insertedAppUsers = new List<ApplicationUserDto>();
        foreach (var applicationUser in applicationUsersToCreate)
        {
            applicationUser.CurrentUser = TestConstants.CurrentUser;
            var insertedAppUser = await _applicationUserLogic.Insert(applicationUser, _applicationLogic);
            
            if (insertedAppUser.Response != null)
            {
                insertedAppUsers.Add(insertedAppUser.Response);

                //change password for users so they can be used for authentication testing...
                await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { ApplicationUserId = insertedAppUser.Response.ApplicationUserId, NewPassword = "Test@1234", CurrentUser = TestConstants.CurrentUser });
            }
        }

        //Create Application Permissions
        var permissionsToCreate = new List<InsertUpdatePermissionRequest>();

        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test Admin Permission 1", Description = "Test Admin Permission 1 Desc." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test Admin Permission 2", Description = "Test Admin Permission 2 Desc." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test Admin Permission 3", Description = "Test Admin Permission 3 Desc." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test Admin Permission 4", Description = "Test Admin Permission 4 Desc." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test Admin Permission 5", Description = "Test Admin Permission 5 Desc." });

        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test Regular User Permission 1", Description = "Test Regular User Permission 1 Desc." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test Regular User Permission 2", Description = "Test Regular User Permission 2 Desc." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test Regular User Permission 3", Description = "Test Regular User Permission 3 Desc." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test Regular User Permission 4", Description = "Test Regular User Permission 4 Desc." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test Regular User Permission 5", Description = "Test Regular User Permission 5 Desc." });

        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test ReadOnly Permission 1", Description = "Test ReadOnly Permission 1 Desc." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test ReadOnly Permission 2", Description = "Test ReadOnly Permission 2 Desc." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test ReadOnly Permission 3", Description = "Test ReadOnly Permission 3 Desc." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test ReadOnly Permission 4", Description = "Test ReadOnly Permission 4 Desc." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Test ReadOnly Permission 5", Description = "Test ReadOnly Permission 5 Desc." });

        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "App User Permission 1", Description = "Specific Permission For App User 1." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "App User Permission 2", Description = "Specific Permission For App User 2." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "App User Permission 3", Description = "Specific Permission For App User 3." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "App User Permission 4", Description = "Specific Permission For App User 4." });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "App User Permission 5", Description = "Specific Permission For App User 5." });

        // permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "PayRoll Admin", Description = "Allows access to payroll administration features." });
        // permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "PayRoll Read Only", Description = "Allows access to payroll administration features in read only mode." });
        // permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "Workout Admin", Description = "Allows access to maintain workout data." });
        // permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = "View Workouts", Description = "Allows read-only access to workout data and reports." });
        
        var insertedAppPermissions = new List<PermissionDto>();
        foreach (var permission in permissionsToCreate)
        {
            permission.CurrentUser = TestConstants.CurrentUser;
            var insertedPermission = await _permissionLogic.Insert(permission, _applicationLogic);

            if (insertedPermission.Response != null)
            {
                insertedAppPermissions.Add(insertedPermission.Response);
            }
        }

        //Create Application Roles
        var rolesToCreate = new List<InsertUpdateRoleRequest>();

        rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = "Admin", Description = "Full Access to all Application Functionality." });
        rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = "User", Description = "Regular App User" });
        rolesToCreate.Add(new InsertUpdateRoleRequest { Active = false, ApplicationId = applicationId, Name = "Read Only", Description = "ReadOnly View of Regular User." });

        // rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = "Admin", Description = "Full Access to all Application Functionality." });
        // rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = "User", Description = "Regular App User" });
        // rolesToCreate.Add(new InsertUpdateRoleRequest { Active = false, ApplicationId = applicationId, Name = "Trainer", Description = "Some features but not all." });

        var insertedAppRoles = new List<RoleDto>();
        foreach (var role in rolesToCreate)
        {
            role.CurrentUser = TestConstants.CurrentUser;
            var insertedRole = await _roleLogic.Insert(role, _applicationLogic);

            if (insertedRole.Response != null)
            {
                insertedAppRoles.Add(insertedRole.Response);
            }
        }

        //Create Role Permissions
        var rolesPermissionsToCreate = new List<InsertUpdateRolePermissionRequest>();

        var adminRoleId = insertedAppRoles.FirstOrDefault(x => x.Name == "Admin").RoleId;
        var adminPermissions = insertedAppPermissions.Where(x => x.Name.Contains("Admin")).ToList();

        foreach (var permission in adminPermissions)
        {
            rolesPermissionsToCreate.Add(new InsertUpdateRolePermissionRequest { Active = true, ApplicationId = applicationId, RoleId = adminRoleId, PermissionId = permission.PermissionId });
        }

        var regularUserRoleId = insertedAppRoles.FirstOrDefault(x => x.Name == "User").RoleId;
        var regularUserPermissions = insertedAppPermissions.Where(x => x.Name.Contains("Regular User")).ToList();

        foreach (var permission in regularUserPermissions)
        {
            rolesPermissionsToCreate.Add(new InsertUpdateRolePermissionRequest { Active = true, ApplicationId = applicationId, RoleId = regularUserRoleId, PermissionId = permission.PermissionId });
        }

        var readOnlyRoleId = insertedAppRoles.FirstOrDefault(x => x.Name == "Read Only").RoleId;
        var readOnlyPermissions = insertedAppPermissions.Where(x => x.Name.Contains("ReadOnly")).ToList();

        foreach (var permission in readOnlyPermissions)
        {
            rolesPermissionsToCreate.Add(new InsertUpdateRolePermissionRequest { Active = true, ApplicationId = applicationId, RoleId = readOnlyRoleId, PermissionId = permission.PermissionId });
        }

        var insertedAppRolePermissions = new List<RolePermissionDto>();
        foreach (var rolePermission in rolesPermissionsToCreate)
        {
            rolePermission.CurrentUser = TestConstants.CurrentUser;
            var insertedRolePermission = await _rolePermissionLogic.Insert(rolePermission, _applicationLogic, _roleLogic, _permissionLogic);

            if (insertedRolePermission.Response != null)
            {
                insertedAppRolePermissions.Add(insertedRolePermission.Response);
            }
        }

        //Create Application User Role
        var applicationUserRolesToCreate = new List<InsertUpdateApplicationUserRoleRequest>();

        var danMaukInserted = insertedAppUsers.FirstOrDefault(x => x.Email == "dmauk@echohealthinc.com");
        applicationUserRolesToCreate.Add(new InsertUpdateApplicationUserRoleRequest { Active = true, ApplicationId = applicationId, RoleId = adminRoleId, ApplicationUserId = danMaukInserted.ApplicationUserId });
        
        var rachelThompsonInserted = insertedAppUsers.FirstOrDefault(x => x.Email == "rthompson@metrohealth.org");
        applicationUserRolesToCreate.Add(new InsertUpdateApplicationUserRoleRequest { Active = true, ApplicationId = applicationId, RoleId = regularUserRoleId, ApplicationUserId = rachelThompsonInserted.ApplicationUserId });

        var lauraMaukInserted = insertedAppUsers.FirstOrDefault(x => x.Email == "PawPatrolOverEverything@gmail.com");
        applicationUserRolesToCreate.Add(new InsertUpdateApplicationUserRoleRequest { Active = true, ApplicationId = applicationId, RoleId = readOnlyRoleId, ApplicationUserId = lauraMaukInserted.ApplicationUserId });

        var insertedapplicationUserRoles = new List<ApplicationUserRoleDto>();
        foreach (var applicationUserRole in applicationUserRolesToCreate)
        {
            applicationUserRole.CurrentUser = TestConstants.CurrentUser;
            var insertedApplicationUserRole = await _applicationUserRoleLogic.Insert(applicationUserRole, _applicationLogic, _applicationUserLogic, _roleLogic);

            if (insertedApplicationUserRole.Response != null)
            {
                insertedapplicationUserRoles.Add(insertedApplicationUserRole.Response);
            }
        }

        //Create Application User Permissions
        var appUserPermissions = insertedAppPermissions.Where(x => x.Name.Contains("App User Permission")).ToList();
        await _applicationUserPermissionLogic.Insert(new InsertUpdateApplicationUserPermissionRequest { Active = true, ApplicationId = applicationId, ApplicationUserId = danMaukInserted.ApplicationUserId, PermissionId = appUserPermissions.FirstOrDefault(x => x.Name == "App User Permission 1").PermissionId, CurrentUser = TestConstants.CurrentUser }, _applicationLogic, _applicationUserLogic, _permissionLogic);
        await _applicationUserPermissionLogic.Insert(new InsertUpdateApplicationUserPermissionRequest { Active = true, ApplicationId = applicationId, ApplicationUserId = danMaukInserted.ApplicationUserId, PermissionId = appUserPermissions.FirstOrDefault(x => x.Name == "App User Permission 2").PermissionId, CurrentUser = TestConstants.CurrentUser }, _applicationLogic, _applicationUserLogic, _permissionLogic);
    }

    private async Task<List<ApplicationDto>> CreateTestApplications()
    {
        var ret = new List<ApplicationDto>();

        var applicationsToCreate = new List<InsertUpdateApplicationRequest>
        {
            new InsertUpdateApplicationRequest { Active = true, Name = "Commission Calculator", Description = "Calculates sales commissions based on tiered rate structures." },
            new InsertUpdateApplicationRequest { Active = true, Name = "Payroll Processing", Description = "Handles employee payroll cycles and direct deposit management." },
            new InsertUpdateApplicationRequest { Active = true, Name = "Invoice Manager", Description = "Tracks and processes vendor and client invoices." },
            new InsertUpdateApplicationRequest { Active = true, Name = "Analytics Dashboard", Description = "Real-time KPI visualizations and business intelligence reporting." },
            new InsertUpdateApplicationRequest { Active = true, Name = "User Access Portal", Description = "Manages user roles, permissions, and authentication workflows." },
            new InsertUpdateApplicationRequest { Active = true, Name = "Audit Log Viewer", Description = "Provides searchable access to system-wide audit trail records." },
            new InsertUpdateApplicationRequest { Active = true, Name = "Notification Service", Description = "Sends email and SMS alerts triggered by configurable system events." },
            new InsertUpdateApplicationRequest { Active = false, Name = "Batch Import Tool", Description = null },
            new InsertUpdateApplicationRequest { Active = false, Name = "Legacy Report Engine", Description = "Deprecated reporting tool replaced by the Analytics Dashboard." },
            new InsertUpdateApplicationRequest { Active = false, Name = "Data Migration Utility", Description = null }
        };

        foreach (var application in applicationsToCreate)
        {
            application.CurrentUser = TestConstants.CurrentUser;
            var insertedApp = await _applicationLogic.Insert(application);
            
            if (insertedApp.Response != null)
            {
                ret.Add(insertedApp.Response);
            }
        }

        return ret;
    }

    private async Task<List<ApplicationUserDto>> CreateTestApplicationUsers(List<ApplicationDto> applications)
    {
        var ret = new List<ApplicationUserDto>();

        var applicationUsersToCreate = new List<InsertUpdateApplicationUserRequest>();

        //Commission Calculator Test Application
        var commissionCalculatorAppId = applications.FirstOrDefault(x => x.Name == "Commission Calculator")?.ApplicationId;
        if (commissionCalculatorAppId != null)
        {
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)commissionCalculatorAppId, Email = "alice.johnson@test.com", FirstName = "Alice", LastName = "Johnson", DateOfBirth = new DateTime(1990, 3, 15) });
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)commissionCalculatorAppId, Email = "bob.smith@test.com", FirstName = "Bob", LastName = "Smith", DateOfBirth = new DateTime(1985, 7, 22) });
        }

        //Payroll Processing Test Application
        var payrollProcessingAppId = applications.FirstOrDefault(x => x.Name == "Payroll Processing")?.ApplicationId;
        if (payrollProcessingAppId != null)
        {
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)payrollProcessingAppId, Email = "carol.white@test.com", FirstName = "Carol", LastName = "White", DateOfBirth = new DateTime(1992, 11, 5) });
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = false, ApplicationId = (int)payrollProcessingAppId, Email = "dan.brown@test.com", FirstName = "Dan", LastName = "Brown", DateOfBirth = new DateTime(1978, 4, 30) });
        }

        //Invoice Manager Test Application
        var invoiceManagerAppId = applications.FirstOrDefault(x => x.Name == "Invoice Manager")?.ApplicationId;
        if (invoiceManagerAppId != null)
        {
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, Email = "eve.davis@test.com", FirstName = "Eve", LastName = "Davis", DateOfBirth = new DateTime(1995, 9, 18) });
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, Email = "frank.miller@test.com", FirstName = "Frank", LastName = "Miller", DateOfBirth = null });
        }

        //Analytics Dashboard Test Application
        var analyticsDashboardAppId = applications.FirstOrDefault(x => x.Name == "Analytics Dashboard")?.ApplicationId;
        if (analyticsDashboardAppId != null)
        {
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)analyticsDashboardAppId, Email = "grace.wilson@test.com", FirstName = "Grace", LastName = "Wilson", DateOfBirth = new DateTime(1988, 1, 25) });
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = false, ApplicationId = (int)analyticsDashboardAppId, Email = "henry.moore@test.com", FirstName = "Henry", LastName = "Moore", DateOfBirth = new DateTime(1970, 6, 12) });
        }

        //User Access Portal Test Application
        var userAccessPortalAppId = applications.FirstOrDefault(x => x.Name == "User Access Portal")?.ApplicationId;
        if (userAccessPortalAppId != null)
        {
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)userAccessPortalAppId, Email = "irene.taylor@test.com", FirstName = "Irene", LastName = "Taylor", DateOfBirth = new DateTime(1993, 8, 8) });
        }

        //Audit Log Viewer Test Application
        var auditLogViewerAppId = applications.FirstOrDefault(x => x.Name == "Audit Log Viewer")?.ApplicationId;
        if (auditLogViewerAppId != null)
        {
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)auditLogViewerAppId, Email = "jack.anderson@test.com", FirstName = null, LastName = null, DateOfBirth = null });
        }

        foreach (var applicationUser in applicationUsersToCreate)
        {
            applicationUser.CurrentUser = TestConstants.CurrentUser;
            var insertedAppUser = await _applicationUserLogic.Insert(applicationUser, _applicationLogic);

            if (insertedAppUser.Response != null)
            {
                ret.Add(insertedAppUser.Response);
            }
        }

        return ret;
    }

    private async Task<List<PermissionDto>> CreateTestPermissions(List<ApplicationDto> applications)
    {
        var ret = new List<PermissionDto>();

        var permissionsToCreate = new List<InsertUpdatePermissionRequest>();

        // Commission Calculator Test Application
        var commissionCalculatorAppId = applications.FirstOrDefault(x => x.Name == "Commission Calculator")?.ApplicationId;
        if (commissionCalculatorAppId != null)
        {
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)commissionCalculatorAppId, Name = "ViewCommissions", Description = "Allows read-only access to commission reports and calculations." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)commissionCalculatorAppId, Name = "ManageCommissions", Description = "Allows creating and editing commission rate structures." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = false, ApplicationId = (int)commissionCalculatorAppId, Name = "DeleteCommissions", Description = "Allows deletion of commission records. Deprecated in favour of soft deletes." });
        }

        // Payroll Processing Test Application
        var payrollProcessingAppId = applications.FirstOrDefault(x => x.Name == "Payroll Processing")?.ApplicationId;
        if (payrollProcessingAppId != null)
        {
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)payrollProcessingAppId, Name = "ViewPayroll", Description = "Allows read-only access to payroll records and summaries." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)payrollProcessingAppId, Name = "ProcessPayroll", Description = "Allows initiating and approving payroll processing cycles." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)payrollProcessingAppId, Name = "ManageDeductions", Description = "Allows managing employee deductions and benefit contributions." });
        }

        // Invoice Manager Test Application
        var invoiceManagerAppId = applications.FirstOrDefault(x => x.Name == "Invoice Manager")?.ApplicationId;
        if (invoiceManagerAppId != null)
        {
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, Name = "ViewInvoices", Description = "Allows read-only access to vendor and client invoices." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, Name = "CreateInvoices", Description = "Allows creating and submitting new invoices." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, Name = "ApproveInvoices", Description = "Allows approving invoices for payment processing." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = false, ApplicationId = (int)invoiceManagerAppId, Name = "VoidInvoices", Description = null });
        }

        // Analytics Dashboard Test Application
        var analyticsDashboardAppId = applications.FirstOrDefault(x => x.Name == "Analytics Dashboard")?.ApplicationId;
        if (analyticsDashboardAppId != null)
        {
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)analyticsDashboardAppId, Name = "ViewReports", Description = "Allows read-only access to all dashboard reports and KPIs." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)analyticsDashboardAppId, Name = "ExportReports", Description = "Allows exporting dashboard data to CSV and PDF formats." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)analyticsDashboardAppId, Name = "ManageDashboards", Description = "Allows creating and configuring custom dashboard layouts." });
        }

        // User Access Portal Test Application
        var userAccessPortalAppId = applications.FirstOrDefault(x => x.Name == "User Access Portal")?.ApplicationId;
        if (userAccessPortalAppId != null)
        {
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)userAccessPortalAppId, Name = "ViewUsers", Description = "Allows read-only access to user accounts and role assignments." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)userAccessPortalAppId, Name = "ManageUsers", Description = "Allows creating, editing, and deactivating user accounts." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)userAccessPortalAppId, Name = "AssignRoles", Description = "Allows assigning and revoking roles and permissions for users." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = false, ApplicationId = (int)userAccessPortalAppId, Name = "ImpersonateUsers", Description = null });
        }

        // Audit Log Viewer Test Application
        var auditLogViewerAppId = applications.FirstOrDefault(x => x.Name == "Audit Log Viewer")?.ApplicationId;
        if (auditLogViewerAppId != null)
        {
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)auditLogViewerAppId, Name = "ViewAuditLogs", Description = "Allows read-only access to the system-wide audit trail." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = (int)auditLogViewerAppId, Name = "ExportAuditLogs", Description = "Allows exporting audit log entries to CSV for compliance reporting." });
            permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = false, ApplicationId = (int)auditLogViewerAppId, Name = "PurgeAuditLogs", Description = "Allows permanent deletion of audit log records. Restricted pending policy review." });
        }

        foreach (var permission in permissionsToCreate)
        {
            permission.CurrentUser = TestConstants.CurrentUser;
            var insertedPermission = await _permissionLogic.Insert(permission, _applicationLogic);

            if (insertedPermission.Response != null)
            {
                ret.Add(insertedPermission.Response);
            }
        }

        return ret;
    }

    private async Task<List<ApplicationUserPermissionDto>> CreateTestApplicationUserPermissions(List<ApplicationDto> applications, List<ApplicationUserDto> applicationUsers, List<PermissionDto> permissions)
    {
        var ret = new List<ApplicationUserPermissionDto>();

        var applicationUserPermissionsToCreate = new List<InsertUpdateApplicationUserPermissionRequest>();

        //Commission Calculator Test Application
        var commissionCalculatorAppId = applications.FirstOrDefault(x => x.Name == "Commission Calculator")?.ApplicationId;
        var commissionCalculatorAppUsers = applicationUsers.Where(x => x.ApplicationId == commissionCalculatorAppId).ToList();
        var commissionCalculatorPermissions = permissions.Where(x => x.ApplicationId == commissionCalculatorAppId).ToList();

        if (commissionCalculatorAppId != null && commissionCalculatorAppUsers != null && commissionCalculatorPermissions != null)
        {
            foreach (var appUser in commissionCalculatorAppUsers)
            {
                foreach (var permission in commissionCalculatorPermissions)
                {
                    applicationUserPermissionsToCreate.Add(new InsertUpdateApplicationUserPermissionRequest { Active = true, ApplicationId = (int)commissionCalculatorAppId, ApplicationUserId = appUser.ApplicationUserId, PermissionId = permission.PermissionId, CurrentUser = TestConstants.CurrentUser });
                }
            }
        }

        //Payroll Processing Test Application
        var payrollProcessingAppId = applications.FirstOrDefault(x => x.Name == "Payroll Processing")?.ApplicationId;
        var payrollProcessingAppUsers = applicationUsers.Where(x => x.ApplicationId == payrollProcessingAppId).ToList();
        var payrollProcessingPermissions = permissions.Where(x => x.ApplicationId == payrollProcessingAppId).ToList();

        if (payrollProcessingAppId != null && payrollProcessingAppUsers != null && payrollProcessingPermissions != null)
        {
            var appUser = payrollProcessingAppUsers[0];
            var permission = payrollProcessingPermissions[0];

            applicationUserPermissionsToCreate.Add(new InsertUpdateApplicationUserPermissionRequest { Active = true, ApplicationId = (int)payrollProcessingAppId, ApplicationUserId = appUser.ApplicationUserId, PermissionId = permission.PermissionId, CurrentUser = TestConstants.CurrentUser });
        }

        //Invoice Manager Test Application
        var invoiceManagerAppId = applications.FirstOrDefault(x => x.Name == "Invoice Manager")?.ApplicationId;
        var invoiceManagerAppUsers = applicationUsers.Where(x => x.ApplicationId == invoiceManagerAppId).ToList();
        var invoiceManagerPermissions = permissions.Where(x => x.ApplicationId == invoiceManagerAppId).ToList();

        if (invoiceManagerAppId != null && invoiceManagerAppUsers != null && invoiceManagerPermissions != null)
        {
            var appUser = invoiceManagerAppUsers[1];
            
            applicationUserPermissionsToCreate.Add(new InsertUpdateApplicationUserPermissionRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, ApplicationUserId = appUser.ApplicationUserId, PermissionId = invoiceManagerPermissions[0].PermissionId, CurrentUser = TestConstants.CurrentUser });
            applicationUserPermissionsToCreate.Add(new InsertUpdateApplicationUserPermissionRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, ApplicationUserId = appUser.ApplicationUserId, PermissionId = invoiceManagerPermissions[1].PermissionId, CurrentUser = TestConstants.CurrentUser });
        }

        //Analytics Dashboard Test Application
        var analyticsDashboardAppId = applications.FirstOrDefault(x => x.Name == "Analytics Dashboard")?.ApplicationId;
        var analyticsDashboardAppUsers = applicationUsers.Where(x => x.ApplicationId == analyticsDashboardAppId).ToList();
        var analyticsDashboardPermissions = permissions.Where(x => x.ApplicationId == analyticsDashboardAppId).ToList();

        if (analyticsDashboardAppId != null && analyticsDashboardAppUsers != null && analyticsDashboardPermissions != null)
        {
            foreach (var appUser in analyticsDashboardAppUsers)
            {
                foreach (var permission in analyticsDashboardPermissions)
                {
                    applicationUserPermissionsToCreate.Add(new InsertUpdateApplicationUserPermissionRequest { Active = true, ApplicationId = (int)analyticsDashboardAppId, ApplicationUserId = appUser.ApplicationUserId, PermissionId = permission.PermissionId, CurrentUser = TestConstants.CurrentUser });
                }
            }
        }

        //User Access Portal Test Application
        var userAccessPortalAppId = applications.FirstOrDefault(x => x.Name == "User Access Portal")?.ApplicationId;
        var userAccessPortalAppUsers = applicationUsers.Where(x => x.ApplicationId == userAccessPortalAppId).ToList();
        var userAccessPortalPermissions = permissions.Where(x => x.ApplicationId == userAccessPortalAppId).ToList();

        if (userAccessPortalAppId != null && userAccessPortalAppUsers != null && userAccessPortalPermissions != null)
        {
            var appUser = userAccessPortalAppUsers[0];
            
            applicationUserPermissionsToCreate.Add(new InsertUpdateApplicationUserPermissionRequest { Active = true, ApplicationId = (int)userAccessPortalAppId, ApplicationUserId = appUser.ApplicationUserId, PermissionId = userAccessPortalPermissions[1].PermissionId, CurrentUser = TestConstants.CurrentUser });
            applicationUserPermissionsToCreate.Add(new InsertUpdateApplicationUserPermissionRequest { Active = true, ApplicationId = (int)userAccessPortalAppId, ApplicationUserId = appUser.ApplicationUserId, PermissionId = userAccessPortalPermissions[2].PermissionId, CurrentUser = TestConstants.CurrentUser });
        }

        //Audit Log Viewer Test Application
        var auditLogViewerAppId = applications.FirstOrDefault(x => x.Name == "Audit Log Viewer")?.ApplicationId;
        var auditLogViewerAppUsers = applicationUsers.Where(x => x.ApplicationId == auditLogViewerAppId).ToList();
        var auditLogViewerPermissions = permissions.Where(x => x.ApplicationId == auditLogViewerAppId).ToList();

        if (auditLogViewerAppId != null && auditLogViewerAppUsers != null && auditLogViewerPermissions != null)
        {
            foreach (var appUser in auditLogViewerAppUsers)
            {
                foreach (var permission in auditLogViewerPermissions)
                {
                    applicationUserPermissionsToCreate.Add(new InsertUpdateApplicationUserPermissionRequest { Active = true, ApplicationId = (int)auditLogViewerAppId, ApplicationUserId = appUser.ApplicationUserId, PermissionId = permission.PermissionId, CurrentUser = TestConstants.CurrentUser });
                }
            }
        }

        foreach (var applicationUserPermission in applicationUserPermissionsToCreate)
        {
            applicationUserPermission.CurrentUser = TestConstants.CurrentUser;
            var insertedAppUser = await _applicationUserPermissionLogic.Insert(applicationUserPermission, _applicationLogic, _applicationUserLogic, _permissionLogic);

            if (insertedAppUser.Response != null)
            {
                ret.Add(insertedAppUser.Response);
            }
        }

        return ret;
    }

    private async Task<List<RoleDto>> CreateTestRoles(List<ApplicationDto> applications)
    {
        var ret = new List<RoleDto>();

        var rolesToCreate = new List<InsertUpdateRoleRequest>();

        // Commission Calculator Test Application
        var commissionCalculatorAppId = applications.FirstOrDefault(x => x.Name == "Commission Calculator")?.ApplicationId;
        if (commissionCalculatorAppId != null)
        {
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)commissionCalculatorAppId, Name = "CommissionViewer", Description = "Read-only access to commission reports and calculation results." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)commissionCalculatorAppId, Name = "CommissionManager", Description = "Full access to manage commission rate structures and overrides." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = false, ApplicationId = (int)commissionCalculatorAppId, Name = "CommissionAdmin", Description = null });
        }

        // Payroll Processing Test Application
        var payrollProcessingAppId = applications.FirstOrDefault(x => x.Name == "Payroll Processing")?.ApplicationId;
        if (payrollProcessingAppId != null)
        {
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)payrollProcessingAppId, Name = "PayrollViewer", Description = "Read-only access to payroll records and employee summaries." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)payrollProcessingAppId, Name = "PayrollProcessor", Description = "Ability to initiate and submit payroll processing cycles." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)payrollProcessingAppId, Name = "PayrollAdmin", Description = "Full access to all payroll functions including approvals and deduction management." });
        }

        // Invoice Manager Test Application
        var invoiceManagerAppId = applications.FirstOrDefault(x => x.Name == "Invoice Manager")?.ApplicationId;
        if (invoiceManagerAppId != null)
        {
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, Name = "InvoiceViewer", Description = "Read-only access to vendor and client invoice records." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, Name = "InvoiceCreator", Description = "Ability to create and submit new invoices for approval." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, Name = "InvoiceApprover", Description = "Ability to review and approve invoices for payment processing." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = false, ApplicationId = (int)invoiceManagerAppId, Name = "InvoiceAdmin", Description = null });
        }

        // Analytics Dashboard Test Application
        var analyticsDashboardAppId = applications.FirstOrDefault(x => x.Name == "Analytics Dashboard")?.ApplicationId;
        if (analyticsDashboardAppId != null)
        {
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)analyticsDashboardAppId, Name = "ReportViewer", Description = "Read-only access to all dashboard reports and KPI metrics." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)analyticsDashboardAppId, Name = "ReportExporter", Description = "Ability to view and export dashboard data to CSV and PDF formats." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)analyticsDashboardAppId, Name = "DashboardAdmin", Description = "Full access to configure and manage custom dashboard layouts." });
        }

        // User Access Portal Test Application
        var userAccessPortalAppId = applications.FirstOrDefault(x => x.Name == "User Access Portal")?.ApplicationId;
        if (userAccessPortalAppId != null)
        {
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)userAccessPortalAppId, Name = "UserViewer", Description = "Read-only access to user accounts and role assignments." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)userAccessPortalAppId, Name = "UserManager", Description = "Ability to create, edit, and deactivate user accounts." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)userAccessPortalAppId, Name = "AccessAdmin", Description = "Full access to manage users, roles, and permission assignments." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = false, ApplicationId = (int)userAccessPortalAppId, Name = "SuperAdmin", Description = null });
        }

        // Audit Log Viewer Test Application
        var auditLogViewerAppId = applications.FirstOrDefault(x => x.Name == "Audit Log Viewer")?.ApplicationId;
        if (auditLogViewerAppId != null)
        {
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)auditLogViewerAppId, Name = "AuditViewer", Description = "Read-only access to the system-wide audit trail." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)auditLogViewerAppId, Name = "AuditExporter", Description = "Ability to view and export audit log entries for compliance reporting." });
            rolesToCreate.Add(new InsertUpdateRoleRequest { Active = false, ApplicationId = (int)auditLogViewerAppId, Name = "AuditAdmin", Description = null });
        }

        foreach (var role in rolesToCreate)
        {
            role.CurrentUser = TestConstants.CurrentUser;
            var insertedRole = await _roleLogic.Insert(role, _applicationLogic);

            if (insertedRole.Response != null)
            {
                ret.Add(insertedRole.Response);
            }
        }

        return ret;
    }

    // private async Task<List<RolePermissionDto>> CreateTestRolePermissions(List<RoleDto> roles, List<PermissionDto> permissions)
    // {
    //     var ret = new List<RolePermissionDto>();

    //     var rolePermissionsToCreate = new List<InsertUpdateRolePermissionRequest>();

    //     // Commission Calculator Test Application
    //     var commissionCalculatorAppId = applications.FirstOrDefault(x => x.Name == "Commission Calculator")?.ApplicationId;
    //     if (commissionCalculatorAppId != null)
    //     {
    //         rolePermissionsToCreate.Add(new InsertUpdateRolePermissionRequest { Active = true, RoleId = (int)commissionCalculatorAppId, PermissionId = (int)commissionCalculatorAppId, Description = "Read-only access to commission reports and calculation results." });
    //         rolePermissionsToCreate.Add(new InsertUpdateRolePermissionRequest { Active = true, RoleId = (int)commissionCalculatorAppId, PermissionId = (int)commissionCalculatorAppId, Description = "Full access to manage commission rate structures and overrides." });
    //         rolePermissionsToCreate.Add(new InsertUpdateRolePermissionRequest { Active = false, RoleId = (int)commissionCalculatorAppId, PermissionId = (int)commissionCalculatorAppId, Description = null });
    //     }

    //     // // Payroll Processing Test Application
    //     // var payrollProcessingAppId = applications.FirstOrDefault(x => x.Name == "Payroll Processing")?.ApplicationId;
    //     // if (payrollProcessingAppId != null)
    //     // {
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)payrollProcessingAppId, Name = "PayrollViewer", Description = "Read-only access to payroll records and employee summaries." });
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)payrollProcessingAppId, Name = "PayrollProcessor", Description = "Ability to initiate and submit payroll processing cycles." });
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)payrollProcessingAppId, Name = "PayrollAdmin", Description = "Full access to all payroll functions including approvals and deduction management." });
    //     // }

    //     // // Invoice Manager Test Application
    //     // var invoiceManagerAppId = applications.FirstOrDefault(x => x.Name == "Invoice Manager")?.ApplicationId;
    //     // if (invoiceManagerAppId != null)
    //     // {
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, Name = "InvoiceViewer", Description = "Read-only access to vendor and client invoice records." });
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, Name = "InvoiceCreator", Description = "Ability to create and submit new invoices for approval." });
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, Name = "InvoiceApprover", Description = "Ability to review and approve invoices for payment processing." });
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = false, ApplicationId = (int)invoiceManagerAppId, Name = "InvoiceAdmin", Description = null });
    //     // }

    //     // // Analytics Dashboard Test Application
    //     // var analyticsDashboardAppId = applications.FirstOrDefault(x => x.Name == "Analytics Dashboard")?.ApplicationId;
    //     // if (analyticsDashboardAppId != null)
    //     // {
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)analyticsDashboardAppId, Name = "ReportViewer", Description = "Read-only access to all dashboard reports and KPI metrics." });
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)analyticsDashboardAppId, Name = "ReportExporter", Description = "Ability to view and export dashboard data to CSV and PDF formats." });
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)analyticsDashboardAppId, Name = "DashboardAdmin", Description = "Full access to configure and manage custom dashboard layouts." });
    //     // }

    //     // // User Access Portal Test Application
    //     // var userAccessPortalAppId = applications.FirstOrDefault(x => x.Name == "User Access Portal")?.ApplicationId;
    //     // if (userAccessPortalAppId != null)
    //     // {
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)userAccessPortalAppId, Name = "UserViewer", Description = "Read-only access to user accounts and role assignments." });
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)userAccessPortalAppId, Name = "UserManager", Description = "Ability to create, edit, and deactivate user accounts." });
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)userAccessPortalAppId, Name = "AccessAdmin", Description = "Full access to manage users, roles, and permission assignments." });
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = false, ApplicationId = (int)userAccessPortalAppId, Name = "SuperAdmin", Description = null });
    //     // }

    //     // // Audit Log Viewer Test Application
    //     // var auditLogViewerAppId = applications.FirstOrDefault(x => x.Name == "Audit Log Viewer")?.ApplicationId;
    //     // if (auditLogViewerAppId != null)
    //     // {
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)auditLogViewerAppId, Name = "AuditViewer", Description = "Read-only access to the system-wide audit trail." });
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = true, ApplicationId = (int)auditLogViewerAppId, Name = "AuditExporter", Description = "Ability to view and export audit log entries for compliance reporting." });
    //     //     rolesToCreate.Add(new InsertUpdateRoleRequest { Active = false, ApplicationId = (int)auditLogViewerAppId, Name = "AuditAdmin", Description = null });
    //     // }

    //     // foreach (var role in rolesToCreate)
    //     // {
    //     //     role.CurrentUser = TestConstants.CurrentUser;
    //     //     var insertedRole = await _roleLogic.Insert(role, _applicationLogic);

    //     //     if (insertedRole.Response != null)
    //     //     {
    //     //         ret.Add(insertedRole.Response);
    //     //     }
    //     // }

    //     return ret;
    // }
}
        