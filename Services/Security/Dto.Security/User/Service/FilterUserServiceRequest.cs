using Dto.Security.User.Logic;
using Shared.Models.Contracts;

namespace Dto.Security.User.Service
{
    public record FilterUserServiceRequest : FilterUserLogicRequest, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
