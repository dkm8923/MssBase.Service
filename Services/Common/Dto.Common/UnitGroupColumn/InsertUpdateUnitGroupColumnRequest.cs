using Shared.Models.Contracts;

namespace Dto.Common.UnitGroupColumn
{
    public record InsertUpdateUnitGroupColumnRequest : ICurrentUser
    {
        public bool Active { get; set; }
        public int UnitId { get; set; }
        public short UnitDefinitionId { get; set; }
        public int SortOrder { get; set; }
        public string CurrentUser { get; set; }
    }
}
