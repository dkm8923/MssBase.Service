using IntegrationTests.Security.Shared.Utilities.Contracts;

namespace IntegrationTests.Security.Shared.Utilities;

public class SecurityTestUtilitiesManager : ISecurityTestUtilitiesManager
{
    private IApplicationUtilities _applicationUtilities;
    private IApplicationUserUtilities _applicationUserUtilities;
    private IRoleUtilities _roleUtilities;
    private IPermissionUtilities _permissionUtilities;

    public SecurityTestUtilitiesManager(
        IApplicationUtilities applicationUtilities,
        IApplicationUserUtilities applicationUserUtilities,
        IRoleUtilities roleUtilities,
        IPermissionUtilities permissionUtilities)
    {
        _applicationUtilities = applicationUtilities;
        _applicationUserUtilities = applicationUserUtilities;
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
