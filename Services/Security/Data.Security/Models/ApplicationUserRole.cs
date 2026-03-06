using Shared.Data.Models;

namespace Data.Security.Models;

public class ApplicationUserRole : AuditableEntity
{
    public int ApplicationUserRoleId { get; set; }
    public int ApplicationUserId { get; set; }
    public int RoleId { get; set; }
    public int ApplicationId { get; set; }
    
    public virtual Application Application { get; set; } = null!;
    public virtual ApplicationUser ApplicationUser { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}
