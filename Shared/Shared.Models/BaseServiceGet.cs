using Shared.Models.Contracts;

namespace Shared.Models
{
    public record BaseServiceGet : BaseLogicGet, IDeleteCache
    {
        public bool DeleteCache { get; set; } = false;
    }
}
