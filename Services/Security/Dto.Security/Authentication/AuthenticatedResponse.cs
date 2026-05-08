namespace Dto.Security.Authentication;

public record AuthenticatedResponse
{
    public string? Token { get; set; }
}
