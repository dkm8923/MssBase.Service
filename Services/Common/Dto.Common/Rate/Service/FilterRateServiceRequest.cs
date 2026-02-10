using Dto.Common.Rate.Logic;
using Shared.Models.Contracts;

namespace Dto.Common.Rate.Service
{
    public record FilterRateServiceRequest : FilterRateLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
