using Shared.Data.Models;
using Shared.Models.Contracts;

namespace Data.Security.Models;

public partial class User : AuditableEntity, IPerson
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? PreferredName { get; set; }
    public string? Suffix { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? TimeZone { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Religion { get; set; }
    public string? Sexuality { get; set; }
    public string? Gender { get; set; }
    public string? SpokenLanguageJson { get; set; }
    public string? PhoneNumberJson { get; set; }
    public string? SocialMediaProfileJson { get; set; }
    public string? AlternateEmailJson { get; set; }

    public virtual UserLogin UserLogin { get; set; } = null!;
    public virtual ICollection<UserRefreshToken> UserRefreshTokens { get; set; } = new List<UserRefreshToken>();
    public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
}
