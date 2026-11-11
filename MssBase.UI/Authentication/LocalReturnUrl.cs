namespace MssBase.UI.Authentication;

public static class LocalReturnUrl
{
    public static string Get(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) &&
            returnUrl.StartsWith("/", StringComparison.Ordinal) &&
            !returnUrl.StartsWith("//", StringComparison.Ordinal) &&
            !returnUrl.StartsWith("/\\", StringComparison.Ordinal) &&
            !returnUrl.Contains('\\') &&
            !returnUrl.Any(char.IsControl))
        {
            return returnUrl;
        }

        return "/";
    }
}