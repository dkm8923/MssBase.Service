namespace Data.Security.Models;

public class UserLogin
{
    public int UserLoginId { get; set; }
    public int UserId { get; set; }
    public string? Password { get; set; }
    public bool PasswordResetRequired { get; set; }
    public DateTime? LastLoginDateTime { get; set; }
    public DateTime? LastPasswordChangeDateTime { get; set; }
    public DateTime? LastLockoutDateTime { get; set; }
    public short? FailedPasswordAttemptCount { get; set; }
    
    public virtual User User { get; set; } = null!;
}
