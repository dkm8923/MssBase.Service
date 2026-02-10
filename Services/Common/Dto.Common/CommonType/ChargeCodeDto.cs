using Shared.Models.Contracts;

namespace Dto.Common.CommonType
{
    public record ChargeCodeDto : ICreateable, IUpdateable
    {
        public int ChargeCodeId { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string ChargeCodeName { get; set; }
    }
}
