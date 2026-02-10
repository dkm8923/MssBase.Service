using System;
using System.Collections.Generic;

namespace Data.Common.Models;

public partial class UnitDefinition
{
    public short UnitDefinitionId { get; set; }

    public string OriginSystem { get; set; } = null!;

    public string SourceColumn { get; set; } = null!;

    public string DestinationColumn { get; set; } = null!;

    public string UserFriendlyDescription { get; set; } = null!;

    public bool UnitValue { get; set; }

    public string UnitValueColumn { get; set; } = null!;

    public bool UnitQty { get; set; }

    public string UnitQtyColumn { get; set; } = null!;

    public bool UnitQuery { get; set; }

    public string UnitQueryColumn { get; set; } = null!;

    public short UnitQueryPosition { get; set; }

    public bool GroupBy { get; set; }

    public string GroupByColumn { get; set; } = null!;

    public bool SupplementalGroupBy { get; set; }

    public string SupplementalGroupByColumn { get; set; } = null!;

    public bool PkgQty { get; set; }

    public string PkgQtyColumn { get; set; } = null!;

    public bool ConditionalAdjustment { get; set; }

    public string ConditionalAdjustmentColumn { get; set; } = null!;

    public string? SqlDataType { get; set; }

    public string? ListObjectName { get; set; }

    public bool UseList { get; set; }

    public bool UsePrimaryKey { get; set; }

    public bool EvaluateAsString { get; set; }

    public bool ExtraCriteria { get; set; }

    public bool Active { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedOn { get; set; }
}
