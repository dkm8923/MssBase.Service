using Shared.Models.Contracts;

namespace Dto.Security.UserPermission
{
    public record InsertUpdateUserPermissionRequest : ICurrentUser
    {
        public bool Active { get; set; }

        public int ApplicationId { get; set; }

        public int ApplicationUserId { get; set; }

        public int PermissionId { get; set; }

        public string CurrentUser { get; set; } = null!;
    }
}
