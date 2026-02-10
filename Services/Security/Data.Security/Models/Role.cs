using System;
using System.Collections.Generic;

namespace Data.Security.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public bool Active { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int ApplicationId { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
