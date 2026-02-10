using Dto.Security.ApplicationUser.Logic;
using Shared.Models.Contracts;

namespace Dto.Security.ApplicationUser.Service
{
    public record FilterApplicationUserServiceRequest : FilterApplicationUserLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
