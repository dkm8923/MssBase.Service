using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;

namespace MssBase.UI.Authentication;

public sealed class ServerSessionStore : ITicketStore
{
    public const string SessionKeyClaimType = "mssbase:session-key";

    private static readonly TimeSpan IdleTimeout = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan AbsoluteTimeout = TimeSpan.FromHours(8);
    private readonly IMemoryCache _cache;
    private readonly IDataProtector _protector;
    private readonly object _sync = new();

    public ServerSessionStore(IMemoryCache cache, IDataProtectionProvider dataProtectionProvider)
    {
        _cache = cache;
        _protector = dataProtectionProvider.CreateProtector("MssBase.UI.ServerSessionStore.v1");
    }

    public Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        AddSessionKeyClaim(ticket, key);
        var session = new Session(_protector.Protect(TicketSerializer.Default.Serialize(ticket)), DateTimeOffset.UtcNow.Add(AbsoluteTimeout));

        lock (_sync)
        {
            Set(key, session);
        }

        return Task.FromResult(key);
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        lock (_sync)
        {
            if (_cache.TryGetValue(key, out Session? session) && session is not null)
            {
                AddSessionKeyClaim(ticket, key);
                Set(key, session with { ProtectedTicket = _protector.Protect(TicketSerializer.Default.Serialize(ticket)) });
            }
        }

        return Task.CompletedTask;
    }

    public Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        lock (_sync)
        {
            if (!_cache.TryGetValue(key, out Session? session) || session is null)
            {
                return Task.FromResult<AuthenticationTicket?>(null);
            }

            try
            {
                return Task.FromResult<AuthenticationTicket?>(TicketSerializer.Default.Deserialize(_protector.Unprotect(session.ProtectedTicket)));
            }
            catch (CryptographicException)
            {
                _cache.Remove(key);
                return Task.FromResult<AuthenticationTicket?>(null);
            }
        }
    }

    public Task RemoveAsync(string key)
    {
        lock (_sync)
        {
            _cache.Remove(key);
        }

        return Task.CompletedTask;
    }

    private void Set(string key, Session session)
    {
        _cache.Set(key, session, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = session.ExpiresAt,
            SlidingExpiration = IdleTimeout
        });
    }

    private static void AddSessionKeyClaim(AuthenticationTicket ticket, string key)
    {
        var identity = ticket.Principal.Identities.FirstOrDefault();
        if (identity is not null && !identity.HasClaim(SessionKeyClaimType, key))
        {
            identity.AddClaim(new Claim(SessionKeyClaimType, key));
        }
    }

    private sealed record Session(byte[] ProtectedTicket, DateTimeOffset ExpiresAt);
}