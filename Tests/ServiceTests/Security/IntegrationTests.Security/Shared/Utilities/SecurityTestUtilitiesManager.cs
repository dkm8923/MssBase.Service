using IntegrationTests.Security.Shared.Utilities.Contracts;

namespace IntegrationTests.Security.Shared.Utilities;

public class SecurityTestUtilitiesManager : ISecurityTestUtilitiesManager
{
    private IApplicationUtilities _applicationUtilities;
    private IApplicationUserUtilities _applicationUserUtilities;
    private IApplicationUserPermissionUtilities _applicationUserPermissionUtilities;
    private IRoleUtilities _roleUtilities;
    private IPermissionUtilities _permissionUtilities;
    
    public SecurityTestUtilitiesManager(
           IApplicationUtilities applicationUtilities,
           IApplicationUserUtilities applicationUserUtilities,
           IApplicationUserPermissionUtilities applicationUserPermissionUtilities,
           IRoleUtilities roleUtilities,
           IPermissionUtilities permissionUtilities
        )
    {
        _applicationUtilities = applicationUtilities;
        _applicationUserUtilities = applicationUserUtilities;
        _applicationUserPermissionUtilities = applicationUserPermissionUtilities;
        _roleUtilities = roleUtilities;
        _permissionUtilities = permissionUtilities;
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
}
