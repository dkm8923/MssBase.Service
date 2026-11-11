using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MssBase.UI.Authentication;

namespace MssBase.UI.HttpClients.Shared;

public sealed class CurrentSessionTokenProvider(
    AuthenticationStateProvider authenticationStateProvider,
    ServerSessionStore sessionStore,
    NavigationManager navigationManager)
{
    private int _sessionExpiryStarted;

    /// <summary>Gets the access token stored in the current authenticated session, if available.</summary>
    public async Task<string?> GetAccessTokenAsync()
    {
        var user = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        var sessionKey = user.FindFirst(ServerSessionStore.SessionKeyClaimType)?.Value;
        if (string.IsNullOrWhiteSpace(sessionKey))
        {
            return null;
        }

        var ticket = await sessionStore.RetrieveAsync(sessionKey);
        return ticket?.Properties.GetTokenValue("access_token");
    }

    /// <summary>Invalidates the current server-side session and forces navigation to sign-in.</summary>
    public async Task ExpireSessionAsync()
    {
        if (Interlocked.Exchange(ref _sessionExpiryStarted, 1) != 0)
        {
            return;
        }

        var user = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        var sessionKey = user.FindFirst(ServerSessionStore.SessionKeyClaimType)?.Value;
        if (!string.IsNullOrWhiteSpace(sessionKey))
        {
            await sessionStore.RemoveAsync(sessionKey);
        }

        navigationManager.NavigateTo("/login?expired=1", forceLoad: true);
    }
}