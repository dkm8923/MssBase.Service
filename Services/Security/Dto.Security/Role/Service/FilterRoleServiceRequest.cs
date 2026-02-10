using Dto.Security.Role.Logic;
using Shared.Models.Contracts;

namespace Dto.Security.Role.Service
{
    public record FilterRoleServiceRequest : FilterRoleLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
