using System;
using System.Collections.Generic;

namespace Data.Common.Models;

public partial class Unit
{
    public int UnitId { get; set; }

    public bool Active { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedOn { get; set; }

    public string? UnitCode { get; set; }

    public string UnitName { get; set; } = null!;

    public string UnitDescription { get; set; } = null!;

    public string OriginSystem { get; set; } = null!;

    public short UnitDefinitionIdUnitQty { get; set; }

    public short UnitDefinitionIdUnitValue { get; set; }

    public string? ValueTypeName { get; set; }

    public string? UnitPrepQuery { get; set; }

    public string? UnitHeaderQuery { get; set; }

    public string? UnitUpdateQuery { get; set; }

    public string? ChargeCode { get; set; }

    //public virtual ICollection<UnitGroupColumn> UnitGroupColumns { get; set; } = new List<UnitGroupColumn>();
}
