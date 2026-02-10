using Dto.Security.UserPermission.Logic;
using Shared.Models.Contracts;

namespace Dto.Security.UserPermission.Service
{
    public record FilterUserPermissionServiceRequest : FilterUserPermissionLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
