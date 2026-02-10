using Shared.Models.Contracts;

namespace Dto.Security.Application
{
    public record InsertUpdateApplicationRequest : ICurrentUser
    {
        public bool Active { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string CurrentUser { get; set; } = null!;
    }
}
