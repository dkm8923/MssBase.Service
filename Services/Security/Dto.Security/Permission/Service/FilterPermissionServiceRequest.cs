using Dto.Security.Permission.Logic;
using Shared.Models.Contracts;

namespace Dto.Security.Permission.Service
{
    public record FilterPermissionServiceRequest : FilterPermissionLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
