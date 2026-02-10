using System;
using System.Collections.Generic;

namespace Data.Common.Models;

public partial class Rate
{
    public long RateId { get; set; }

    public bool Active { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedOn { get; set; }

    public int UnitId { get; set; }

    public decimal RateAmt { get; set; }

    public int MinLimit { get; set; }

    public int MaxLimit { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string UnitValue { get; set; } = null!;

    public string Facility { get; set; } = null!;
}
