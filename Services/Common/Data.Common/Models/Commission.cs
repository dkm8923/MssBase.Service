using System;
using System.Collections.Generic;

namespace Data.Common.Models;

public partial class Commission
{
    public long CommissionId { get; set; }

    public bool Active { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedOn { get; set; }

    public bool? CommissionProcessed { get; set; }

    public DateOnly CycleEndDate { get; set; }

    public string Facility { get; set; } = null!;

    public string RegionalServiceProvider { get; set; } = null!;

    public int? VehicleCount { get; set; }

    public int? CompletedRouteCount { get; set; }

    public int? ExpectedRouteCount { get; set; }

    public int? DroppedRouteCount { get; set; }

    public int? QuickCoverageCount { get; set; }

    public int? CyclePackageCount { get; set; }

    public int? MileageCount { get; set; }

    public string? PackageIncentiveTier { get; set; }
}
