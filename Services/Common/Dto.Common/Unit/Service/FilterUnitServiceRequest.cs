using Dto.Common.Unit.Logic;
using Shared.Models.Contracts;

namespace Dto.Common.Unit.Service
{
    public record FilterUnitServiceRequest : FilterUnitLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
