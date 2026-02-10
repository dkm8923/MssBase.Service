using Shared.Models.Contracts;

namespace Dto.Common.Rate
{
    public record RateDto : ICreateable, IUpdateable
    {
        public long RateId { get; set; }

        public bool Active { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int UnitId { get; set; }

        public decimal RateAmt { get; set; }

        public int MinLimit { get; set; }

        public int MaxLimit { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string UnitValue { get; set; }

        public string Facility { get; set; }
    }
}
