namespace Data.Security.Models;

public class UserLogin
{
    public int UserLoginId { get; set; }
    public int UserId { get; set; }
    public string? Password { get; set; }
    public bool PasswordResetRequired { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime? LastPasswordChangeDate { get; set; }
    public DateTime? LastLockoutDate { get; set; }
    public short? FailedPasswordAttemptCount { get; set; }
    
    public virtual User User { get; set; } = null!;
}
