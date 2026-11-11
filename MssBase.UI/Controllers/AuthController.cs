using System.Security.Claims;
using Dto.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MssBase.UI.Authentication;
using MssBase.UI.Configuration;
using Shared.Models;

namespace MssBase.UI.Controllers;

[AllowAnonymous]
[Route("auth")]
public sealed class AuthController(
    IHttpClientFactory httpClientFactory,
    IOptions<SecurityApiOptions> securityApiOptions) : Controller
{
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        [FromForm] string? email,
        [FromForm] string? password,
        [FromForm] string? returnUrl,
        CancellationToken cancellationToken)
    {
        email = email?.Trim();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
        {
            return Redirect("/login?error=1");
        }

        try
        {
            using var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(securityApiOptions.Value.BaseAddress, UriKind.Absolute);
            using var response = await client.PostAsJsonAsync(
                "Authentication/Authenticate",
                new AuthenticationRequest
                {
                    ApplicationName = securityApiOptions.Value.ApplicationName,
                    Email = email,
                    Password = password
                },
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return Redirect("/login?error=1");
            }

            var result = await response.Content.ReadFromJsonAsync<ErrorValidationResult<AuthenticationResponse>>(
                cancellationToken: cancellationToken);
            var tokens = result?.Response;

            if (result is null || result.Errors.Count > 0 ||
                string.IsNullOrWhiteSpace(tokens?.Token) || string.IsNullOrWhiteSpace(tokens.RefreshToken))
            {
                return Redirect("/login?error=1");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, email),
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var properties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                AllowRefresh = true
            };
            properties.StoreTokens(new[]
            {
                new AuthenticationToken { Name = "access_token", Value = tokens.Token },
                new AuthenticationToken { Name = "refresh_token", Value = tokens.RefreshToken }
            });

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties);

            return LocalRedirect(LocalReturnUrl.Get(returnUrl));
        }
        catch (HttpRequestException)
        {
            return Redirect("/login?error=1");
        }
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return LocalRedirect("/login");
    }
}