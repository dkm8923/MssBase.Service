namespace Dto.Security.Authentication;

public record ResetPasswordResponse
{
    public string NewPassword { get; init; }
}