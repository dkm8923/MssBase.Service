using Shared.Data.Models;

namespace Data.Security.Models;

public partial class User : AuditableEntity
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Password { get; set; }
    
    public virtual UserLogin UserLogin { get; set; } = null!;
    public virtual ICollection<UserRefreshToken> UserRefreshTokens { get; set; } = new List<UserRefreshToken>();
    public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
}
