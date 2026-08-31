using Shared.Data.Models;

namespace Data.Security.Models;

public partial class ApplicationUser : AuditableEntity
{
    public int ApplicationUserId { get; set; }
    public int UserId { get; set; }
    public int ApplicationId { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual Application Application { get; set; } = null!;
    public virtual ICollection<ApplicationUserPermission> ApplicationUserPermissions { get; set; } = new List<ApplicationUserPermission>();
    public virtual ICollection<ApplicationUserRole> ApplicationUserRoles { get; set; } = new List<ApplicationUserRole>();
}
