namespace Data.Security.Models;

public class UserRefreshToken
{
    public int UserRefreshTokenId { get; set; }
    public int UserId { get; set; }
    public int ApplicationId { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    public virtual User User { get; set; } = null!;
    public virtual Application Application { get; set; } = null!;
}
