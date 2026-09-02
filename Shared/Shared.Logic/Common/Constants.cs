namespace Shared.Logic.Common
{
    //Place all shared constants here...
    public static class Constants
    {
        public const string ApplicationName = "MssBase.Service";
        public const string ApplicationsClaim = "applications";
        public const string RolesClaim = "roles";
        public const string PermissionsClaim = "permissions";
        
        public static class EntityFieldNames
        {
            public const string User = "User";
            public const string UserId = "UserId";
            public const string Application = "Application";
            public const string ApplicationId = "ApplicationId";
            public const string ApplicationName = "ApplicationName";
            public const string ApplicationUser = "ApplicationUser";
            public const string ApplicationUsers = "ApplicationUsers";
            public const string ApplicationUserId = "ApplicationUserId";
            public const string ApplicationUserPermission = "ApplicationUserPermission";
            public const string ApplicationUserPermissionId = "ApplicationUserPermissionId";
            public const string ApplicationUserPermissions = "ApplicationUserPermissions";
            public const string ApplicationUserRoleId = "ApplicationUserRoleId";
            public const string ApplicationUserRole = "ApplicationUserRole";
            public const string ApplicationUserRoles = "ApplicationUserRoles";
            public const string RolePermission = "RolePermission";
            public const string RolePermissionId = "RolePermissionId";
            public const string Role = "Role";
            public const string RoleId = "RoleId";
            public const string Permission = "Permission";
            public const string PermissionId = "PermissionId";
            public const string Title = "Title";
            public const string FirstName = "FirstName";
            public const string MiddleName = "MiddleName";
            public const string LastName = "LastName";
            public const string PreferredName = "PreferredName";
            public const string Suffix = "Suffix";
            public const string TimeZone = "TimeZone";
            public const string Name = "Name";
            public const string Email = "Email";
            public const string Description = "Description";
            public const string DateOfBirth = "DateOfBirth";
            public const string Password = "Password";
            public const string NewPassword = "NewPassword";
            public const string Token = "Token";
            public const string ChangePassword = "ChangePassword";
            public const string RefreshToken = "RefreshToken";
            public const string Authentication = "Authentication";
            public const string CurrentUser = "CurrentUser";
        }

        public static class AuditLogLogTypes
        {
            public const string Update = "Update";
            public const string Delete = "Delete";
        }
    }
}
