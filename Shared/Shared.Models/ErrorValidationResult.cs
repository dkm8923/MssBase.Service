namespace Shared.Models
{
    public record ErrorValidationResult
    {
        public Dictionary<string, List<string>> Errors { get; set; } = new();
    }

    public record ErrorValidationResult<TResponse> : ErrorValidationResult
    {
        public TResponse? Response { get; set; }
    }
}
