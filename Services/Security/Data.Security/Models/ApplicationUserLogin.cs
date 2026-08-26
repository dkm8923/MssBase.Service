namespace Data.Security.Models;

public class ApplicationUserLogin
{
    public int ApplicationUserLoginId { get; set; }
    public int ApplicationId { get; set; }
    public int ApplicationUserId { get; set; }
    public string? Password { get; set; }
    public bool PasswordResetRequired { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public DateTime? LastPasswordChangeDate { get; set; }

    public DateTime? LastLockoutDate { get; set; }

    public short? FailedPasswordAttemptCount { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public virtual Application Application { get; set; } = null!;
    public virtual ApplicationUser ApplicationUser { get; set; } = null!;
}
