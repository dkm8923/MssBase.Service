using Dto.Security.ApplicationUserPermission.Logic;
using Shared.Models.Contracts;

namespace Dto.Security.ApplicationUserPermission.Service
{
    public record FilterApplicationUserPermissionServiceRequest : FilterApplicationUserPermissionLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
