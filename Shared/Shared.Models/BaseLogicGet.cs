using Shared.Models.Contracts;

namespace Shared.Models
{
    public record BaseLogicGet : ICurrentUser, IIncludeRelated
    {
        public bool IncludeInactive { get; set; } = false;
        public bool IncludeRelated { get; set; } = false;
        public string CurrentUser { get; set; } = "MssBase.Service";
    }
}
