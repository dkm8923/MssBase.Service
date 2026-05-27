using IntegrationTests.Security.Shared.Utilities.Contracts;

namespace IntegrationTests.Security.Shared.Utilities;

public class AuthenticationUtilities : IAuthenticationUtilities
{
    public Dictionary<string, List<string>> GetExpectedAccountLockedErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Authentication", new List<string> { "Account is locked due to too many failed login attempts. Please try again after 60 minutes!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedInvalidCredentialsErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Authentication", new List<string> { "Invalid email address or password!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationName", new List<string> { "ApplicationName cannot exceed 64 characters!" } },
            { "Email", new List<string> { "Email cannot exceed 128 characters!" } },
            { "Password", new List<string> { "Password cannot exceed 64 characters!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationName", new List<string> { "ApplicationName is a required field!" } },
            { "Email", new List<string> { "Email is a required field!" } },
            { "Password", new List<string> { "Password is a required field!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedPasswordChangeRequiredErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Authentication", new List<string> { "Password change is required. Please update your password!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedInvalidApplicationIdFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationName", new List<string> { "Record does not exist for specified ApplicationName!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRefreshTokenRequiredFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Token", new List<string> { "Token is a required field!" } },
            { "RefreshToken", new List<string> { "RefreshToken is a required field!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRefreshTokenMaxLengthFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "RefreshToken", new List<string> { "RefreshToken cannot exceed 2048 characters!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRefreshTokenUserNotFoundErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "RefreshToken", new List<string> { "User Not Found!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRefreshTokenInvalidAuthTokenErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "RefreshToken", new List<string> { "Invalid Token!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRefreshTokenInvalidRefreshTokenErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "RefreshToken", new List<string> { "Invalid Refresh Token!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRefreshTokenExpiredErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "RefreshToken", new List<string> { "Refresh Token Expired!" } }
        };
    }
}
