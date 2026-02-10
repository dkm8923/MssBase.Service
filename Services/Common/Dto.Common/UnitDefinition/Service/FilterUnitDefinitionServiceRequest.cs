using Dto.Common.UnitDefinition.Logic;
using Shared.Models.Contracts;

namespace Dto.Common.UnitDefinition.Service
{
    public record FilterUnitDefinitionServiceRequest : FilterUnitDefinitionLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
