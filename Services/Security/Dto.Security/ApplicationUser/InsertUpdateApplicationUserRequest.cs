using Shared.Models.Contracts;

namespace Dto.Security.ApplicationUser
{
    public record InsertUpdateApplicationUserRequest : ICurrentUser
    {
        public int UserId { get; set; }
        public int ApplicationId { get; set; }
        public bool Active { get; set; }
        public string CurrentUser { get; set; } = null!;
    }
}
