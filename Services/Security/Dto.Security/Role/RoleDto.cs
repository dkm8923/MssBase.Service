using Shared.Models.Contracts;

namespace Dto.Security.Role
{
    public record RoleDto// : ICreateable, IUpdateable
    {
        public int RoleId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedBy { get; set; }

        public bool Active { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int ApplicationId { get; set; }
    }
}
