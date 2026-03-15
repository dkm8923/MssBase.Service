namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface ISecurityTestUtilitiesManager
{
    public IApplicationUtilities Application { get; }
    public IApplicationUserUtilities ApplicationUser { get; }
    public IApplicationUserPermissionUtilities ApplicationUserPermission { get; }
    public IApplicationUserRoleUtilities ApplicationUserRole { get; }
    public IRoleUtilities Role { get; }
    public IPermissionUtilities Permission { get; }
    public IRolePermissionUtilities RolePermission { get; }
}
