namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface ISecurityTestUtilitiesManager
{
    public IApplicationUtilities Application { get; }
    public IApplicationUserUtilities ApplicationUser { get; }
    public IRoleUtilities Role { get; }
    public IPermissionUtilities Permission { get; }
}
