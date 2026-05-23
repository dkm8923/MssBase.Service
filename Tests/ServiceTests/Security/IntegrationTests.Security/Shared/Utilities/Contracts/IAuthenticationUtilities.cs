using Dto.Security.ApplicationUser;

namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface IAuthenticationUtilities
{
    public Dictionary<string, List<string>> GetExpectedInvalidCredentialsErrors();
    public Dictionary<string, List<string>> GetExpectedAccountLockedErrors();
    public Dictionary<string, List<string>> GetExpectedPasswordChangeRequiredErrors();
    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedInvalidApplicationIdFieldErrors();

}
