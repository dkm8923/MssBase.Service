using Shared.Models;

namespace Dto.Common.Rate.Logic
{
    public record FilterRateLogicRequest : BaseLogicGet
    {
        public string? CreatedBy { get; set; }
        public DateOnly? CreatedOnDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateOnly? UpdatedOnDate { get; set; }
        public List<long>? RateIds { get; set; }
        public int? UnitId { get; set; }

        public decimal? RateAmt { get; set; }

        public int? MinLimit { get; set; }

        public int? MaxLimit { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? UnitValue { get; set; }

        public string? Facility { get; set; }
    }
}
