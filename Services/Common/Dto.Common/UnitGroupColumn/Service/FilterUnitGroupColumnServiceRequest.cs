using Dto.Common.UnitGroupColumn.Logic;
using Shared.Models.Contracts;

namespace Dto.Common.UnitGroupColumn.Service
{
    public record FilterUnitGroupColumnServiceRequest : FilterUnitGroupColumnLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
