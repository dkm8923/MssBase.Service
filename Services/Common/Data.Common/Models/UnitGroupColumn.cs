using System;
using System.Collections.Generic;

namespace Data.Common.Models;

public partial class UnitGroupColumn
{
    public int UnitGroupColumnId { get; set; }

    public int UnitId { get; set; }

    public short UnitDefinitionId { get; set; }

    public int SortOrder { get; set; }

    public bool Active { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedOn { get; set; }

    public virtual Unit Unit { get; set; } = null!;
}
