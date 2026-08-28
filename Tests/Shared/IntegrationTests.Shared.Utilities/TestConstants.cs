using System;

namespace IntegrationTests.Shared.Utilities;

public static class TestConstants
{
    public const string CurrentUser = "IntegrationTest";
    public const string SpecificCurrentUserForInsert = "IntegrationTestInsert";
    public const string SpecificCurrentUserForUpdate = "IntegrationTestUpdate";
    public const string DefaultTestUserEmail = "IntegrationTest@example.com";
    public const string DefaultTestUserPassword = "!0TestPassword1230!";
    public const string DefaultTestUserApplicationName = "MSS Security";
    public const string LogTypeUpdate = "Update";
    public const string LogTypeDelete = "Delete";
    public const string ReferenceTypeApplication = "Application";
    public const string ReferenceTypeUser = "User";
    public const string ReferenceTypeApplicationUser = "ApplicationUser";
    public const string ReferenceTypePermission = "Permission";
    public const string ReferenceTypeRole = "Role";
    public const string ReferenceTypeApplicationUserPermission = "ApplicationUserPermission";
    public const string ReferenceTypeApplicationUserRole = "ApplicationUserRole";
    public const string ReferenceTypeRolePermission = "RolePermission";
}
