using Dto.Security.ApplicationUserRole.Logic;
using Shared.Models.Contracts;

namespace Dto.Security.ApplicationUserRole.Service
{
    public record FilterApplicationUserRoleServiceRequest : FilterApplicationUserRoleLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
