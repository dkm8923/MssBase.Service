using Dto.Security.RolePermission.Logic;
using Shared.Models.Contracts;

namespace Dto.Security.RolePermission.Service
{
    public record FilterRolePermissionServiceRequest : FilterRolePermissionLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
