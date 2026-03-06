using Shared.Data.Models;

namespace Data.Security.Models;

public partial class Application : AuditableEntity
{
    public int ApplicationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public virtual ICollection<ApplicationUserPermission> ApplicationUserPermissions { get; set; } = new List<ApplicationUserPermission>();
    public virtual ICollection<ApplicationUserRole> ApplicationUserRoles { get; set; } = new List<ApplicationUserRole>();
}
