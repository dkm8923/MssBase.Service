using Shared.Models.Contracts;

namespace Dto.Common.UnitGroupColumn
{
    public class UnitGroupColumnDto : ICreateable, IUpdateable
    {
        public int UnitGroupColumnId { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; } = null!;
        public DateTime? UpdatedOn { get; set; }
        public int UnitId { get; set; }
        public short UnitDefinitionId { get; set; }
        public int SortOrder { get; set; }
    }
}
