using Shared.Models.Contracts;

namespace Dto.Common.CommonType
{
    public record OriginSystemDto : ICreateable, IUpdateable
    {
        public int OriginSystemId { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string OriginSystemName { get; set; }
    }
}
