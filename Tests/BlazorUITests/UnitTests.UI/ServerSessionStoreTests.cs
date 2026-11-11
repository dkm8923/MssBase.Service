using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using MssBase.UI.Authentication;
using Xunit;

namespace UnitTests.UI;

public class ServerSessionStoreTests
{
    /// <summary>Verifies stored tickets receive distinct opaque keys and can be retrieved.</summary>
    [Fact]
    public async Task Store_GeneratesDistinctKeysAndPreservesTickets()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var store = new ServerSessionStore(cache, new EphemeralDataProtectionProvider());

        var firstKey = await store.StoreAsync(CreateTicket("first"));
        var secondKey = await store.StoreAsync(CreateTicket("second"));

        Assert.NotEqual(firstKey, secondKey);
        Assert.Equal(64, firstKey.Length);
        Assert.Equal("first", (await store.RetrieveAsync(firstKey))?.Principal.Identity?.Name);
        Assert.Equal("second", (await store.RetrieveAsync(secondKey))?.Principal.Identity?.Name);
        Assert.Equal(firstKey, (await store.RetrieveAsync(firstKey))?.Principal.FindFirst(ServerSessionStore.SessionKeyClaimType)?.Value);
        Assert.Null(await store.RetrieveAsync("unknown"));
    }

    /// <summary>Verifies renewal updates an existing ticket but does not restore a removed session.</summary>
    [Fact]
    public async Task Renew_UpdatesExistingSessionButDoesNotRestoreRemovedSession()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var store = new ServerSessionStore(cache, new EphemeralDataProtectionProvider());
        var key = await store.StoreAsync(CreateTicket("first"));

        await store.RenewAsync(key, CreateTicket("updated"));
        Assert.Equal("updated", (await store.RetrieveAsync(key))?.Principal.Identity?.Name);

        await store.RemoveAsync(key);
        await store.RenewAsync(key, CreateTicket("restored"));
        Assert.Null(await store.RetrieveAsync(key));
    }

    /// <summary>Verifies tickets protected with a different Data Protection key are rejected.</summary>
    [Fact]
    public async Task Retrieve_RejectsTicketsEncryptedWithDifferentKey()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var firstStore = new ServerSessionStore(cache, new EphemeralDataProtectionProvider());
        var secondStore = new ServerSessionStore(cache, new EphemeralDataProtectionProvider());
        var key = await firstStore.StoreAsync(CreateTicket("first"));

        Assert.Null(await secondStore.RetrieveAsync(key));
        Assert.Null(await firstStore.RetrieveAsync(key));
    }

    private static AuthenticationTicket CreateTicket(string name)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, name)], "test"));
        return new AuthenticationTicket(principal, "test");
    }
}