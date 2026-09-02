namespace MssBase.Service.Shared.Authorization;

public static class UserApiRoles
{
    #region Application Roles

    public const string ApplicationAdmin = "ApplicationAdmin";
    public const string ApplicationReadOnly = "ApplicationReadOnly";
    
    #endregion

    #region User Roles

    public const string UserAdmin = "UserAdmin";
    public const string UserReadOnly = "UserReadOnly";

    #endregion

    #region ApplicationUser Roles

    public const string ApplicationUserAdmin = "ApplicationUserAdmin";
    public const string ApplicationUserReadOnly = "ApplicationUserReadOnly";
    
    #endregion

    #region ApplicationUserPermission Roles

    public const string ApplicationUserPermissionAdmin = "ApplicationUserPermissionAdmin";
    public const string ApplicationUserPermissionReadOnly = "ApplicationUserPermissionReadOnly";
    
    #endregion

    #region ApplicationUserRole Roles

    public const string ApplicationUserRoleAdmin = "ApplicationUserRoleAdmin";
    public const string ApplicationUserRoleReadOnly = "ApplicationUserRoleReadOnly";
    
    #endregion

    #region Permission Roles

    public const string PermissionAdmin = "PermissionAdmin";
    public const string PermissionReadOnly = "PermissionReadOnly";
    
    #endregion

    #region Role Roles

    public const string RoleAdmin = "RoleAdmin";
    public const string RoleReadOnly = "RoleReadOnly";

    #endregion

    #region RolePermission Roles

    public const string RolePermissionAdmin = "RolePermissionAdmin";
    public const string RolePermissionReadOnly = "RolePermissionReadOnly";

    #endregion
}