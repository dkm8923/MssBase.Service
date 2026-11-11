# Authentication Implementation Notes

Status as of 2026-11-11. This document explains the current authentication-related Git changes and why they exist. Git does not record per-file authorship for uncommitted changes, so this describes the contents of the current diff rather than claiming every line was authored in one session.

## How the Flow Works

1. The login page posts the email and password to the UI server, not directly from the browser to the Security API.
2. The UI server validates antiforgery, calls `Authentication/Authenticate`, and stores the returned access and refresh tokens in the protected server-side authentication ticket.
3. The browser receives a secure, HTTP-only cookie containing an opaque session key. The ticket itself is held in the UI process's memory cache.
4. Protected UI routes require authentication. UI service clients retrieve the access token for the current circuit's session and attach it to each API request.
5. A service `401` removes that UI session and sends the user to sign in again. Logout clears the local cookie and ticket.

This keeps JWTs out of browser storage and out of the cookie payload. It does not yet refresh access tokens or revoke the API refresh token on logout.

## Changed Files

### Startup, routing, and navigation

- `MssBase.Service.slnx`: adds the UI test project to the solution so it can be built and tested with the rest of the repository.
- `MssBase.UI/Program.cs`: configures cookie authentication, the server-side ticket store, authorization, and MVC controllers. It maps attribute-routed controllers; auth form actions are implemented in `Controllers/AuthController.cs`.
- `MssBase.UI/Controllers/AuthController.cs`: owns the `/auth/login` and `/auth/logout` POST routes. MVC antiforgery filters protect both actions. Login calls the configured Security API, stores tokens in the ticket, and issues the cookie; logout clears the local sign-in. These actions do not implement refresh-token renewal or backend token revocation.
- `MssBase.UI/Configuration/SecurityApiOptions.cs`: adds a configurable Security API application name, needed by the authentication request.
- `MssBase.UI/appsettings.json`: sets the application name to `MSS Security` and retains the local Security API base address.
- `MssBase.UI/Components/_Imports.razor`: imports Blazor authorization types for use in routed components.
- `MssBase.UI/Components/Routes.razor`: uses `AuthorizeRouteView` so route authorization metadata is enforced and provides an authorizing/not-authorized state.
- `MssBase.UI/Components/Pages/_Imports.razor`: applies `[Authorize]` to all components under the pages folder, so new app pages are protected by default.
- `MssBase.UI/Components/Pages/Home.razor`: inherits the pages-folder authorization policy.
- `MssBase.UI/Components/Pages/Permission/PermissionCreateEdit.razor`: inherits the pages-folder authorization policy.
- `MssBase.UI/Components/Pages/Permission/PermissionList.razor`: inherits the pages-folder authorization policy.
- `MssBase.UI/Components/Pages/Role/RoleList.razor`: inherits the pages-folder authorization policy.
- `MssBase.UI/Components/Pages/NotFound.razor`: opts out of the pages-folder policy because the generic 404 endpoint must be accessible during status-code re-execution and contains no protected application data.
- `MssBase.UI/Components/Layout/MainLayout.razor`: shows sign-in when anonymous and an antiforgery-protected sign-out form when authenticated.
- `MssBase.UI/Components/Layout/AuthLayout.razor`: provides MudBlazor providers for the login screen without the app bar or navigation drawer.
- `MssBase.UI/Components/Layout/AuthLayout.razor.css`: centers the auth page and fills the viewport.
- `MssBase.UI/Components/Pages/Login.razor`: opts out of the pages-folder authorization policy, then provides the email/password form, generic failure and expired-session messages, antiforgery token, and return-URL field. It selects `AuthLayout` so no application navigation appears on the login screen.
- `MssBase.UI/Components/Pages/Login.razor.css`: styles the login fields and focus state.
- `MssBase.UI/Authentication/LocalReturnUrl.cs`: accepts only local return paths to prevent an external open redirect after login.

### Server-side sessions and API requests

- `MssBase.UI/Authentication/ServerSessionStore.cs`: implements ASP.NET Core's `ITicketStore`. It generates random session keys, protects serialized tickets with Data Protection, and applies 30-minute idle and 8-hour absolute timeouts. Its `IMemoryCache` is process-local, so sessions are lost on restart and are not shared across instances; Redis and a shared Data Protection key ring remain future work.
- `MssBase.UI/HttpClients/Shared/CurrentSessionTokenProvider.cs`: resolves the current circuit's opaque session key to its server-side ticket and access token. On expiry it removes that ticket and forces a login redirect.
- `MssBase.UI/HttpClients/Shared/SessionHttpClient.cs`: attaches the current access token to each outgoing request rather than storing it on a shared client/handler. On `401`, it disposes the response, removes the current session, and requests a fresh login.
- `MssBase.UI/HttpClients/Security/ApplicationHttpClient.cs`: routes application API requests through `SessionHttpClient`.
- `MssBase.UI/HttpClients/Security/PermissionHttpClient.cs`: routes permission API requests through `SessionHttpClient`.
- `MssBase.UI/HttpClients/Security/RoleHttpClient.cs`: routes role API requests through `SessionHttpClient`.
- `MssBase.UI/HttpClients/Shared/HttpClientUtils.cs`: updates the shared GET-result helper to use the session-aware client.
- `MssBase.UI/HttpClients/Shared/BearerTokenHandler.cs` (deleted): removes the previous handler that embedded a fixed bearer token. Requests now use the signed-in session's token.

### Tests and tracking

- `Tests/UnitTests.UI/UnitTests.UI.csproj`: defines the UI xUnit project and adds `Microsoft.AspNetCore.Mvc.Testing` for HTTP-level tests against the real UI middleware.
- `Tests/UnitTests.UI/ServerSessionStoreTests.cs`: tests distinct opaque keys, ticket retrieval/renewal/removal, and rejection of tickets protected with a different Data Protection key.
- `Tests/UnitTests.UI/AuthenticationSecurityTests.cs`: tests session-specific bearer tokens, `401` session invalidation, return-URL validation, protected-route challenges, antiforgery rejection for login/logout, and a simulated login/logout round trip. The round trip checks secure/HTTP-only cookie flags and that token values are not in the cookie.
- `docs/SECURITY_AUTHENTICATION_TODO.md`: tracks which original plan items are complete, deferred, or still outstanding. Step 1 remains explicitly skipped; step 5 is only partially complete; step 6 has not started.

## Current Limits and Follow-up

- The session store is in-memory only. Move it to Redis when desired and persist/share Data Protection keys for multi-instance deployment.
- Expired access tokens currently require a new login. Coordinated refresh and retry depend on secure refresh-token rotation, which is part of the deferred service hardening.
- Logout clears the UI session but does not revoke the Security API refresh token. The revocation endpoint needs the deferred security changes first.
- Service-side rate limiting, generic authentication responses, refresh replay/race handling, disabled-account invalidation, and penetration testing are not established by these UI tests.
- The UI expects the Security API at `SecurityApi:BaseAddress` (`http://localhost:5217/api/Security/` in the current settings). The latest terminal output showed `Connection refused (localhost:5217)`, which means the API was not listening at that address; start it or configure the correct reachable address before testing a real login.
- The UI build passed with existing warnings. The UI test suite passed all 16 tests. Test restore reported moderate and high vulnerability advisories for `System.Security.Cryptography.Pkcs` 9.0.4; remediation is tracked in the security TODO.
