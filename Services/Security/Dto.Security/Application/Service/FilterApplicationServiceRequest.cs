using Dto.Security.Application.Logic;
using Shared.Models.Contracts;

namespace Dto.Security.Application.Service
{
    public record FilterApplicationServiceRequest : FilterApplicationLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
