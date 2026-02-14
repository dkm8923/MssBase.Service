//using Dto.Common.UnitGroupColumn;
using Shared.Models.Contracts;

namespace Dto.Common.Unit
{
    public record UnitDto : ICreateable, IUpdateable
    {
        public int UnitId { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? UnitCode { get; set; }
        public string UnitName { get; set; }
        public string UnitDescription { get; set; }
        public string OriginSystem { get; set; }
        public short UnitDefinitionIdUnitQty { get; set; }
        public short UnitDefinitionIdUnitValue { get; set; }
        public string? ValueTypeName { get; set; }
        public string? UnitPrepQuery { get; set; }
        public string? UnitHeaderQuery { get; set; }
        public string? UnitUpdateQuery { get; set; }
        public string? ChargeCode { get; set; }
        //public List<UnitGroupColumnDto>? UnitGroupColumns { get; set; }
    }
}
