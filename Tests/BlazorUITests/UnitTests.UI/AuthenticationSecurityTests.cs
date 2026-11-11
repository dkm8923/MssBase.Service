using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using MssBase.UI.Authentication;
using MssBase.UI.HttpClients.Shared;
using Xunit;

namespace UnitTests.UI;

public class AuthenticationSecurityTests
{
    /// <summary>Verifies each authenticated session sends only its own access token.</summary>
    [Fact]
    public async Task SessionHttpClient_UsesOnlyTheCurrentSessionsAccessToken()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var store = new ServerSessionStore(cache, new EphemeralDataProtectionProvider());
        var firstTicket = CreateTicket("first", "first-access-token");
        var secondTicket = CreateTicket("second", "second-access-token");
        await store.StoreAsync(firstTicket);
        await store.StoreAsync(secondTicket);

        var firstRequest = await SendRequestAsync(firstTicket.Principal, store);
        var secondRequest = await SendRequestAsync(secondTicket.Principal, store);

        Assert.Equal("first-access-token", firstRequest);
        Assert.Equal("second-access-token", secondRequest);
    }

    /// <summary>Verifies an API 401 removes the current session and navigates to sign-in.</summary>
    [Fact]
    public async Task SessionHttpClient_401RemovesCurrentSessionAndNavigatesToLogin()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var store = new ServerSessionStore(cache, new EphemeralDataProtectionProvider());
        var ticket = CreateTicket("user", "access-token");
        var sessionKey = await store.StoreAsync(ticket);
        var navigationManager = new TestNavigationManager();
        var tokenProvider = new CurrentSessionTokenProvider(
            new FixedAuthenticationStateProvider(ticket.Principal),
            store,
            navigationManager);
        var httpClient = new HttpClient(new StubHttpMessageHandler(
            _ => new HttpResponseMessage(HttpStatusCode.Unauthorized)))
        {
            BaseAddress = new Uri("https://security.example/api/")
        };
        var sessionHttpClient = new SessionHttpClient(httpClient, tokenProvider);

        using var response = await sessionHttpClient.GetAsync("permissions");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Null(await store.RetrieveAsync(sessionKey));
        Assert.Equal("http://localhost/login?expired=1", navigationManager.NavigatedTo);
    }

    /// <summary>Verifies the login endpoint rejects a POST without an antiforgery token.</summary>
    [Fact]
    public async Task LoginEndpoint_RejectsPostWithoutAntiforgeryToken()
    {
        using var factory = new UiApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var response = await client.PostAsync(
            "/auth/login",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["email"] = "user@example.com",
                ["password"] = "password"
            }));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>Verifies the logout endpoint rejects a POST without an antiforgery token.</summary>
    [Fact]
    public async Task LogoutEndpoint_RejectsPostWithoutAntiforgeryToken()
    {
        using var factory = new UiApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var response = await client.PostAsync("/auth/logout", new FormUrlEncodedContent([]));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>Verifies login and logout keep tokens out of the cookie and clear the session.</summary>
    [Fact]
    public async Task LoginAndLogout_KeepTokensOutOfCookieAndClearSession()
    {
        using var factory = new UiApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        using var loginPage = await client.GetAsync("/login");
        var loginHtml = await loginPage.Content.ReadAsStringAsync();
        var tokenInput = Regex.Match(loginHtml, "<input[^>]*name=\"__RequestVerificationToken\"[^>]*>");
        Assert.True(tokenInput.Success, "The login form should include an antiforgery token.");
        var requestToken = Regex.Match(tokenInput.Value, "value=\"([^\"]+)\"").Groups[1].Value;
        Assert.False(string.IsNullOrWhiteSpace(requestToken));

        using var loginResponse = await client.PostAsync("/auth/login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = requestToken,
            ["email"] = "user@example.com",
            ["password"] = "password",
            ["returnUrl"] = "/permission-list"
        }));

        Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);
        var loginLocation = loginResponse.Headers.Location;
        Assert.Equal("/permission-list", loginLocation?.IsAbsoluteUri == true
            ? loginLocation.PathAndQuery
            : loginLocation?.OriginalString);
        var authCookie = Assert.Single(
            loginResponse.Headers.GetValues("Set-Cookie"),
            value => value.StartsWith("__Host-MssBase.Auth=", StringComparison.Ordinal));
        Assert.Contains("httponly", authCookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("secure", authCookie, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("access-token-secret", authCookie, StringComparison.Ordinal);
        Assert.DoesNotContain("refresh-token-secret", authCookie, StringComparison.Ordinal);

        using var authenticatedPage = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, authenticatedPage.StatusCode);

        using var authenticatedLoginPage = await client.GetAsync("/login");
        var authenticatedHtml = await authenticatedLoginPage.Content.ReadAsStringAsync();
        var logoutTokenInput = Regex.Match(authenticatedHtml, "<input[^>]*name=\"__RequestVerificationToken\"[^>]*>");
        Assert.True(logoutTokenInput.Success, "The authenticated page should include an antiforgery token.");
        var logoutRequestToken = Regex.Match(logoutTokenInput.Value, "value=\"([^\"]+)\"").Groups[1].Value;

        using var logoutResponse = await client.PostAsync("/auth/logout", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = logoutRequestToken
        }));

        Assert.Equal(HttpStatusCode.Redirect, logoutResponse.StatusCode);
        Assert.Contains(logoutResponse.Headers.GetValues("Set-Cookie"), value =>
            value.StartsWith("__Host-MssBase.Auth=", StringComparison.Ordinal) &&
            value.Contains("expires=", StringComparison.OrdinalIgnoreCase));

        using var anonymousPage = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.Redirect, anonymousPage.StatusCode);
    }

    /// <summary>Verifies an anonymous request to a protected page redirects to sign-in.</summary>
    [Fact]
    public async Task ProtectedPage_RedirectsAnonymousUserToLogin()
    {
        using var factory = new UiApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var response = await client.GetAsync("/permission-list");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/login?ReturnUrl=", response.Headers.Location?.PathAndQuery);
    }

    /// <summary>Verifies valid local return paths are preserved after authentication.</summary>
    [Theory]
    [InlineData("/permission-list", "/permission-list")]
    [InlineData("/permission-list?includeInactive=true", "/permission-list?includeInactive=true")]
    public void LocalReturnUrl_AcceptsLocalPaths(string candidate, string expected)
    {
        Assert.Equal(expected, LocalReturnUrl.Get(candidate));
    }

    /// <summary>Verifies external and malformed return paths fall back to the home page.</summary>
    [Theory]
    [InlineData("https://attacker.example", "/")]
    [InlineData("//attacker.example", "/")]
    [InlineData("/\\attacker.example", "/")]
    [InlineData("not-a-path", "/")]
    [InlineData("/path\r\nLocation: https://attacker.example", "/")]
    public void LocalReturnUrl_RejectsExternalOrMalformedPaths(string candidate, string expected)
    {
        Assert.Equal(expected, LocalReturnUrl.Get(candidate));
    }

    private static async Task<string?> SendRequestAsync(ClaimsPrincipal user, ServerSessionStore store)
    {
        string? token = null;
        var tokenProvider = new CurrentSessionTokenProvider(
            new FixedAuthenticationStateProvider(user),
            store,
            new TestNavigationManager());
        var httpClient = new HttpClient(new StubHttpMessageHandler(request =>
        {
            token = request.Headers.Authorization?.Parameter;
            return new HttpResponseMessage(HttpStatusCode.OK);
        }))
        {
            BaseAddress = new Uri("https://security.example/api/")
        };
        var sessionHttpClient = new SessionHttpClient(httpClient, tokenProvider);

        using var response = await sessionHttpClient.GetAsync("permissions");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return token;
    }

    private static AuthenticationTicket CreateTicket(string userName, string accessToken)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, userName)],
            "test"));
        var properties = new AuthenticationProperties();
        properties.StoreTokens([new AuthenticationToken { Name = "access_token", Value = accessToken }]);
        return new AuthenticationTicket(principal, properties, "test");
    }

    private sealed class FixedAuthenticationStateProvider(ClaimsPrincipal user) : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(new AuthenticationState(user));
    }

    private sealed class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager()
        {
            Initialize("http://localhost/", "http://localhost/");
        }

        public string? NavigatedTo { get; private set; }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            NavigatedTo = ToAbsoluteUri(uri).ToString();
        }
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(respond(request));
    }

    private sealed class UiApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var contentRoot = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(typeof(Program).Assembly.Location)!,
                "../../../"));
            builder.UseContentRoot(contentRoot);
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["SecurityApi:BaseAddress"] = "https://security-api.example/api/Security/",
                    ["SecurityApi:ApplicationName"] = "MSS Security"
                }));
            builder.ConfigureTestServices(services =>
                services.AddHttpClient(string.Empty)
                    .ConfigurePrimaryHttpMessageHandler(() => new AuthenticationApiHandler()));
        }
    }

    private sealed class AuthenticationApiHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """{"response":{"token":"access-token-secret","refreshToken":"refresh-token-secret"},"errors":{}}""",
                    Encoding.UTF8,
                    "application/json")
            });
    }
}