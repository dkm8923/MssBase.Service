namespace MssBase.Service.Shared.Authorization;

public static class UserApiPermissions
{
    //Application Permissions
    #region Application Permissions

    public const string ApplicationRead = "ApplicationRead";
    public const string ApplicationInsert = "ApplicationInsert";
    public const string ApplicationUpdate = "ApplicationUpdate";
    public const string ApplicationDelete = "ApplicationDelete";

    #endregion
    
    #region User Permissions

    public const string UserRead = "UserRead";
    public const string UserPasswordChangeHistoryRead = "UserPasswordChangeHistoryRead";
    public const string UserInsert = "UserInsert";
    public const string UserUpdate = "UserUpdate";
    public const string UserDelete = "UserDelete";
    public const string UserResetPassword = "UserResetPassword";
    public const string UserChangePassword = "UserChangePassword";

    #endregion

    #region Application User Permissions

    public const string ApplicationUserRead = "ApplicationUserRead";
    public const string ApplicationUserPasswordChangeHistoryRead = "ApplicationUserPasswordChangeHistoryRead";
    public const string ApplicationUserInsert = "ApplicationUserInsert";
    public const string ApplicationUserUpdate = "ApplicationUserUpdate";
    public const string ApplicationUserDelete = "ApplicationUserDelete";
    public const string ApplicationUserResetPassword = "ApplicationUserResetPassword";
    public const string ApplicationUserChangePassword = "ApplicationUserChangePassword";

    #endregion

    #region Application User Permission Permissions

    public const string ApplicationUserPermissionRead = "ApplicationUserPermissionRead";
    public const string ApplicationUserPermissionInsert = "ApplicationUserPermissionInsert";
    public const string ApplicationUserPermissionUpdate = "ApplicationUserPermissionUpdate";
    public const string ApplicationUserPermissionDelete = "ApplicationUserPermissionDelete";

    #endregion

    #region Application User Role Permissions

    public const string ApplicationUserRoleRead = "ApplicationUserRoleRead";
    public const string ApplicationUserRoleInsert = "ApplicationUserRoleInsert";
    public const string ApplicationUserRoleUpdate = "ApplicationUserRoleUpdate";
    public const string ApplicationUserRoleDelete = "ApplicationUserRoleDelete";

    #endregion

    #region Permission Permissions

    public const string PermissionRead = "PermissionRead";
    public const string PermissionInsert = "PermissionInsert";
    public const string PermissionUpdate = "PermissionUpdate";
    public const string PermissionDelete = "PermissionDelete";

    #endregion

    #region Role Permissions

    public const string RoleRead = "RoleRead";
    public const string RoleInsert = "RoleInsert";
    public const string RoleUpdate = "RoleUpdate";
    public const string RoleDelete = "RoleDelete";

    #endregion

    #region RolePermission Permissions

    public const string RolePermissionRead = "RolePermissionRead";
    public const string RolePermissionInsert = "RolePermissionInsert";
    public const string RolePermissionUpdate = "RolePermissionUpdate";
    public const string RolePermissionDelete = "RolePermissionDelete";

    #endregion

    #region Common Permissions

    #endregion
}