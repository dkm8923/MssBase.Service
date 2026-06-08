namespace Dto.Security.Authentication;

public class PasswordValidationConfig
{
    public int RequiredLength { get; set; }
    public int MaxLength { get; set; }
    public bool RequireNonAlphanumeric { get; set; }
    public bool RequireLowercase { get; set; }
    public bool RequireUppercase { get; set; }
    public bool RequireDigit { get; set; }
    public bool RequirePasswordHistoryCheck { get; set; }
    public int RequirePasswordHistoryCheckOldPasswordCount { get; set; }
}
