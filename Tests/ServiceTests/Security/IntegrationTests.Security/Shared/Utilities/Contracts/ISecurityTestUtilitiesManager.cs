namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface ISecurityTestUtilitiesManager
{
    public IApplicationUtilities Application { get; }
    public IApplicationUserUtilities ApplicationUser { get; }
}
