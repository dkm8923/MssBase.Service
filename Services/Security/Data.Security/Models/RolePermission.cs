using System;
using System.Collections.Generic;

namespace Data.Security.Models;

public partial class RolePermission
{
    public int RolePermissionId { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public bool Active { get; set; }

    public int ApplicationId { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
