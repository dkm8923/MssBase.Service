using Dto.Common.CommonType.Logic;
using Shared.Models.Contracts;

namespace Dto.Common.CommonType.Service
{
    public record FilterCommonTypeServiceRequest : FilterCommonTypeLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
