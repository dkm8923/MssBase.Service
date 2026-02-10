using Shared.Models.Contracts;

namespace Dto.Security.Role
{
    public record InsertUpdateRoleRequest : ICurrentUser
    {
        public bool Active { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int ApplicationId { get; set; }

        public string CurrentUser { get; set; } = null!;
    }
}
