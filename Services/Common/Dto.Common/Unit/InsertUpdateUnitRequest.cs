using Shared.Models.Contracts;

namespace Dto.Common.Unit
{
    public record InsertUpdateUnitRequest : ICurrentUser
    {
        public int UnitId { get; set; }
        public bool Active { get; set; }
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
        public string CurrentUser { get; set; }
    }
}
