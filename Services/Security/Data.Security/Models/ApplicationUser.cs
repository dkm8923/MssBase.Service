using Shared.Data.Models;

namespace Data.Security.Models;

public partial class ApplicationUser : AuditableEntity
{
    public int ApplicationUserId { get; set; }

    public string Email { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Password { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public DateTime? LastPasswordChangeDate { get; set; }

    public DateTime? LastLockoutDate { get; set; }

    public short? FailedPasswordAttemptCount { get; set; }

    public int ApplicationId { get; set; }

    public virtual Application Application { get; set; } = null!;
    public virtual ICollection<ApplicationUserPermission> ApplicationUserPermissions { get; set; } = new List<ApplicationUserPermission>();
}
