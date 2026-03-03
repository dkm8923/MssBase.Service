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

    [Fact]
    public async Task Seed_SecurityDB()
    {
        // Create test data for manual testing purposes
        await ClearAllSecurityTestTableData();

        var applications = await CreateTestApplications();
        
        await CreateTestApplicationUsers(applications);
        await CreateTestPermissions(applications);
        await CreateTestRoles(applications);

        // Assert
        1.Should().Be(1);
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
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)commissionCalculatorAppId, Email = "alice.johnson@test.com", FirstName = "Alice", LastName = "Johnson", DateOfBirth = new DateTime(1990, 3, 15), Password = "P@ssw0rd1!", LastLoginDate = DateTime.UtcNow.AddDays(-1), LastPasswordChangeDate = DateTime.UtcNow.AddMonths(-2), LastLockoutDate = null, FailedPasswordAttemptCount = 0 });
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)commissionCalculatorAppId, Email = "bob.smith@test.com", FirstName = "Bob", LastName = "Smith", DateOfBirth = new DateTime(1985, 7, 22), Password = "P@ssw0rd2!", LastLoginDate = DateTime.UtcNow.AddDays(-3), LastPasswordChangeDate = DateTime.UtcNow.AddMonths(-1), LastLockoutDate = null, FailedPasswordAttemptCount = 0 });
        }

        //Payroll Processing Test Application
        var payrollProcessingAppId = applications.FirstOrDefault(x => x.Name == "Payroll Processing")?.ApplicationId;
        if (payrollProcessingAppId != null)
        {
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)payrollProcessingAppId, Email = "carol.white@test.com", FirstName = "Carol", LastName = "White", DateOfBirth = new DateTime(1992, 11, 5), Password = "P@ssw0rd3!", LastLoginDate = DateTime.UtcNow.AddHours(-5), LastPasswordChangeDate = DateTime.UtcNow.AddMonths(-3), LastLockoutDate = null, FailedPasswordAttemptCount = 0 });
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = false, ApplicationId = (int)payrollProcessingAppId, Email = "dan.brown@test.com", FirstName = "Dan", LastName = "Brown", DateOfBirth = new DateTime(1978, 4, 30), Password = "P@ssw0rd4!", LastLoginDate = DateTime.UtcNow.AddDays(-30), LastPasswordChangeDate = DateTime.UtcNow.AddMonths(-6), LastLockoutDate = DateTime.UtcNow.AddDays(-30), FailedPasswordAttemptCount = 5 });
        }

        //Invoice Manager Test Application
        var invoiceManagerAppId = applications.FirstOrDefault(x => x.Name == "Invoice Manager")?.ApplicationId;
        if (invoiceManagerAppId != null)
        {
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, Email = "eve.davis@test.com", FirstName = "Eve", LastName = "Davis", DateOfBirth = new DateTime(1995, 9, 18), Password = "P@ssw0rd5!", LastLoginDate = DateTime.UtcNow.AddDays(-2), LastPasswordChangeDate = DateTime.UtcNow.AddMonths(-1), LastLockoutDate = null, FailedPasswordAttemptCount = 1 });
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)invoiceManagerAppId, Email = "frank.miller@test.com", FirstName = "Frank", LastName = "Miller", DateOfBirth = null, Password = "P@ssw0rd6!", LastLoginDate = null, LastPasswordChangeDate = null, LastLockoutDate = null, FailedPasswordAttemptCount = 0 });
        }

        //Analytics Dashboard Test Application
        var analyticsDashboardAppId = applications.FirstOrDefault(x => x.Name == "Analytics Dashboard")?.ApplicationId;
        if (analyticsDashboardAppId != null)
        {
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)analyticsDashboardAppId, Email = "grace.wilson@test.com", FirstName = "Grace", LastName = "Wilson", DateOfBirth = new DateTime(1988, 1, 25), Password = "P@ssw0rd7!", LastLoginDate = DateTime.UtcNow.AddHours(-1), LastPasswordChangeDate = DateTime.UtcNow.AddDays(-10), LastLockoutDate = null, FailedPasswordAttemptCount = 0 });
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = false, ApplicationId = (int)analyticsDashboardAppId, Email = "henry.moore@test.com", FirstName = "Henry", LastName = "Moore", DateOfBirth = new DateTime(1970, 6, 12), Password = null, LastLoginDate = DateTime.UtcNow.AddDays(-90), LastPasswordChangeDate = DateTime.UtcNow.AddYears(-1), LastLockoutDate = DateTime.UtcNow.AddDays(-60), FailedPasswordAttemptCount = 3 });
        }

        //User Access Portal Test Application
        var userAccessPortalAppId = applications.FirstOrDefault(x => x.Name == "User Access Portal")?.ApplicationId;
        if (userAccessPortalAppId != null)
        {
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)userAccessPortalAppId, Email = "irene.taylor@test.com", FirstName = "Irene", LastName = "Taylor", DateOfBirth = new DateTime(1993, 8, 8), Password = "P@ssw0rd9!", LastLoginDate = DateTime.UtcNow, LastPasswordChangeDate = DateTime.UtcNow.AddDays(-5), LastLockoutDate = null, FailedPasswordAttemptCount = 0 });
        }

        //Audit Log Viewer Test Application
        var auditLogViewerAppId = applications.FirstOrDefault(x => x.Name == "Audit Log Viewer")?.ApplicationId;
        if (auditLogViewerAppId != null)
        {
            applicationUsersToCreate.Add(new InsertUpdateApplicationUserRequest { Active = true, ApplicationId = (int)auditLogViewerAppId, Email = "jack.anderson@test.com", FirstName = null, LastName = null, DateOfBirth = null, Password = "P@ssw0rd10!", LastLoginDate = DateTime.UtcNow.AddDays(-7), LastPasswordChangeDate = DateTime.UtcNow.AddMonths(-4), LastLockoutDate = null, FailedPasswordAttemptCount = 2 });
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
}
        