using Shared.Models.Contracts;

namespace Dto.Security.Application
{
    public record ApplicationDto// : ICreateable, IUpdateable
    {
        public int ApplicationId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedBy { get; set; }

        public bool Active { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
