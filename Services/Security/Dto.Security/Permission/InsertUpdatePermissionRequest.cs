using Shared.Models.Contracts;

namespace Dto.Security.Permission
{
    public record InsertUpdatePermissionRequest : ICurrentUser
    {
        public bool Active { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int ApplicationId { get; set; }

        public string CurrentUser { get; set; } = null!;
    }
}
