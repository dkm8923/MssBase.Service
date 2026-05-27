namespace IntegrationTests.Security.Shared.Utilities.Contracts;

public interface IAuthenticationUtilities
{
    public Dictionary<string, List<string>> GetExpectedInvalidCredentialsErrors();
    public Dictionary<string, List<string>> GetExpectedAccountLockedErrors();
    public Dictionary<string, List<string>> GetExpectedPasswordChangeRequiredErrors();
    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedInvalidApplicationIdFieldErrors();
    public Dictionary<string, List<string>> GetExpectedRefreshTokenRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedRefreshTokenMaxLengthFieldErrors();
    public Dictionary<string, List<string>> GetExpectedRefreshTokenUserNotFoundErrors();
    public Dictionary<string, List<string>> GetExpectedRefreshTokenInvalidAuthTokenErrors();
    public Dictionary<string, List<string>> GetExpectedRefreshTokenInvalidRefreshTokenErrors();
    public Dictionary<string, List<string>> GetExpectedRefreshTokenExpiredErrors();
    public Dictionary<string, List<string>> GetExpectedRevokeTokenRequiredFieldErrors();
}
