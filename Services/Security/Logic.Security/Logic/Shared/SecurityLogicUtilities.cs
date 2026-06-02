using Microsoft.AspNetCore.Identity;

public static class SecurityLogicUtilities
{
    /// <summary>
    /// Verifies the provided password against the stored password hash using ASP.NET Core Identity's PasswordHasher. Returns true if the password is valid, false otherwise.
    /// </summary>
    /// <param name="passwordHash"></param>
    /// <param name="providedPassword"></param>
    /// <returns></returns>
    public static bool VerifyPasswordMatchesHash(string passwordHash, string providedPassword)
    {
        var hasher = new PasswordHasher<object>();
        var result = hasher.VerifyHashedPassword(user: null, hashedPassword: passwordHash, providedPassword: providedPassword);
        bool isValidPassword = result == PasswordVerificationResult.Success ||
        result == PasswordVerificationResult.SuccessRehashNeeded;
        return isValidPassword;
    }      
}