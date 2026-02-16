using IntegrationTests.Security.Shared.Utilities.Contracts;

namespace IntegrationTests.Security.Shared.Utilities;

public class SecurityTestUtilitiesManager : ISecurityTestUtilitiesManager
{
    private IApplicationUtilities _applicationUtilities;
    private IApplicationUserUtilities _applicationUserUtilities;
    
    public SecurityTestUtilitiesManager(
        IApplicationUtilities applicationUtilities,
        IApplicationUserUtilities applicationUserUtilities)
    {
        _applicationUtilities = applicationUtilities;
        _applicationUserUtilities = applicationUserUtilities;
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
}
