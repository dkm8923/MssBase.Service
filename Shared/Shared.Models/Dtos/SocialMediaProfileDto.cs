namespace Shared.Models.Dtos;

public record SocialMediaProfileDto
{
    public string Platform { get; set; }
    public string? Url { get; set; }
    public string? UserName { get; set; }
}
