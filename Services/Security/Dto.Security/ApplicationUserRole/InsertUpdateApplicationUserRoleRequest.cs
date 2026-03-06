using Shared.Models.Contracts;

namespace Dto.Security.ApplicationUserRole
{
    public record InsertUpdateApplicationUserRoleRequest : ICurrentUser
    {
        public bool Active { get; set; }

        public int ApplicationId { get; set; }

        public int ApplicationUserId { get; set; }

        public int RoleId { get; set; }

        public string CurrentUser { get; set; } = null!;
    }
}
