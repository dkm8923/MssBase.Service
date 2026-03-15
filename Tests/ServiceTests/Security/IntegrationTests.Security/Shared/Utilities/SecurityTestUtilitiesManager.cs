using IntegrationTests.Security.Shared.Utilities.Contracts;

namespace IntegrationTests.Security.Shared.Utilities;

public class SecurityTestUtilitiesManager : ISecurityTestUtilitiesManager
{
    private IApplicationUtilities _applicationUtilities;
    private IApplicationUserUtilities _applicationUserUtilities;
    private IApplicationUserPermissionUtilities _applicationUserPermissionUtilities;
    private IApplicationUserRoleUtilities _applicationUserRoleUtilities;
    private IRoleUtilities _roleUtilities;
    private IPermissionUtilities _permissionUtilities;
    private IRolePermissionUtilities _rolePermissionUtilities;
    
    public SecurityTestUtilitiesManager(
           IApplicationUtilities applicationUtilities,
           IApplicationUserUtilities applicationUserUtilities,
           IApplicationUserPermissionUtilities applicationUserPermissionUtilities,
           IApplicationUserRoleUtilities applicationUserRoleUtilities,
           IRoleUtilities roleUtilities,
           IPermissionUtilities permissionUtilities,
           IRolePermissionUtilities rolePermissionUtilities
        )
    {
        _applicationUtilities = applicationUtilities;
        _applicationUserUtilities = applicationUserUtilities;
        _applicationUserPermissionUtilities = applicationUserPermissionUtilities;
        _applicationUserRoleUtilities = applicationUserRoleUtilities;
        _roleUtilities = roleUtilities;
        _permissionUtilities = permissionUtilities;
        _rolePermissionUtilities = rolePermissionUtilities;
    }

    public IApplicationUtilities Application
    {
        get
        {
            return _applicationUtilities;
        }
    }

    public IApplicationUserUtilities ApplicationUser
    {
        get
        {
            return _applicationUserUtilities;
        }
    }

    public IApplicationUserPermissionUtilities ApplicationUserPermission
    {
        get
        {
            return _applicationUserPermissionUtilities;
        }
    }

    public IApplicationUserRoleUtilities ApplicationUserRole
    {
        get
        {
            return _applicationUserRoleUtilities;
        }
    }

    public IRoleUtilities Role
    {
        get
        {
            return _roleUtilities;
        }
    }

    public IPermissionUtilities Permission
    {
        get
        {
            return _permissionUtilities;
        }
    }

    public IRolePermissionUtilities RolePermission
    {
        get
        {
            return _rolePermissionUtilities;
        }
    }
}
