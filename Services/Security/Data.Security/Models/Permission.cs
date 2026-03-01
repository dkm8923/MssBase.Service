using Shared.Data.Models;

namespace Data.Security.Models;

public partial class Permission : AuditableEntity
{
    public int PermissionId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int ApplicationId { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public virtual ICollection<ApplicationUserPermission> ApplicationUserPermissions { get; set; } = new List<ApplicationUserPermission>();
}
