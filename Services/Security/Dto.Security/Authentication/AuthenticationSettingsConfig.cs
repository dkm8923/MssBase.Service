namespace Dto.Security.Authentication;

public class AuthenticationSettingsConfig
{
    public int MaxFailedPasswordAttemptCount { get; set; }
    public int LockoutDurationInMinutes { get; set; }
    public int PasswordExpiryInDays { get; set; }
}
