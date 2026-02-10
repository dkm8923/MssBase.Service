using Shared.Models;

namespace Dto.Common.UnitGroupColumn.Logic
{
    public record FilterUnitGroupColumnLogicRequest : BaseLogicGet
    {
        public string CreatedBy { get; set; }
        public DateOnly? CreatedOnDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateOnly? UpdatedOnDate { get; set; }
        public int? UnitId { get; set; }
        public short? UnitDefinitionId { get; set; }
        public int? SortOrder { get; set; }
    }
}
