# Authentication Security TODO

Status as of 2026-11-10. This checklist tracks the agreed authentication plan. Step 5 is still in progress; step 6 has not started.

## Step 1: Security-service hardening (skipped for now)

The user explicitly deferred this step. Do not treat it as completed.

- [ ] Secure refresh-token revocation: require authorization, derive identity/application from validated claims, and avoid accepting another user's email from the request.
- [ ] Remove token values from logs and assess/revoke any exposed tokens.
- [ ] Hash stored refresh tokens; plan migration for existing token data.
- [ ] Implement atomic refresh-token rotation, replay detection, and per-session revocation.
- [ ] Review refresh-token expiration and UTC handling.
- [ ] Add authentication throttling/rate limits.
- [ ] Review password-hash verification/rehashing, lockout concurrency, generic login errors, JWT validation, and secret management.
- [ ] Disable or redesign the current forgot-password behavior before exposing recovery; it must not reset a password without verified ownership.

## Step 2: Server-side session (partially complete)

- [x] Generate an opaque random session key and keep authentication tickets server-side.
- [x] Protect serialized tickets with ASP.NET Core Data Protection.
- [x] Enforce idle and absolute session expiry.
- [ ] Replace process-local `IMemoryCache` with Redis when ready; use a distinct key prefix for UI sessions.
- [ ] Persist/share the Data Protection key ring for deployments with multiple UI instances.
- [ ] Implement coordinated refresh so concurrent requests cannot race token renewal. Refresh-token rotation itself depends on step 1.
- [ ] Verify login rotates the session key and cannot preserve a pre-login session identifier.

## Step 3: Login and logout (implemented; backend revocation deferred)

- [x] Email/password login through the UI server to the Security API.
- [x] Keep access and refresh tokens in the protected server-side ticket, not browser storage or the cookie payload.
- [x] Validate antiforgery tokens on login and logout; validate local return URLs.
- [x] Clear the local UI session on logout.
- [ ] Revoke the backend refresh token during logout after step 1 defines a secure revocation contract.

## Step 4: UI authorization (implemented; follow-up behavior depends on service work)

- [x] Require authentication on protected UI routes.
- [x] Attach the current session's access token to outgoing service requests without shared handler state.
- [x] Clear the local session and require sign-in again on API `401` responses.
- [x] Remove the hard-coded bearer token handler.
- [ ] Add coordinated refresh and retry on expired access tokens after secure refresh rotation is implemented.
- [ ] Confirm disabled/revoked accounts and tokens are rejected by the Security API; the UI currently reacts to `401` but cannot enforce backend invalidation itself.

## Step 5: Security tests and verification (in progress)

UI test suite currently passes (16 tests).

- [x] Cover per-session token isolation, `401` invalidation, and local return-URL validation.
- [x] Cover anonymous access to protected routes.
- [x] Cover missing antiforgery tokens on login/logout.
- [x] Cover simulated login/logout, secure cookie flags, absence of plaintext tokens in the cookie, and cookie clearing.
- [ ] Add service integration tests for generic login errors/account enumeration and lockout behavior; review existing tests before adding duplicates.
- [ ] Add tests for authentication throttling/rate limits when implemented.
- [ ] Add refresh-token expiration, rotation, replay, and concurrent-refresh tests when implemented.
- [ ] Verify service authorization rejects anonymous requests and still enforces permission policies after replacing the UI bearer token.
- [ ] Test logout/revocation, disabled-account handling, and remote token revocation once the backend contract exists.
- [ ] Verify sensitive tokens are absent from application logs and diagnostic output.
- [ ] Run dependency/security scanning and remediate the reported `System.Security.Cryptography.Pkcs` 9.0.4 advisories.
- [ ] Arrange an independent penetration test after the backend hardening and deployment configuration are complete.

## Step 6: Password recovery and MFA (not started)

- [ ] Implement short-lived, single-use password-reset links with generic responses and rate limits.
- [ ] Revoke active sessions after a password reset.
- [ ] Add MFA; prefer phishing-resistant passkeys where feasible.

## Notes

- A penetration-test pass cannot be guaranteed in advance; results depend on the threat model, deployment, configuration, and test scope.
- Do not mark deferred items complete merely because UI-side behavior is tested; service-side guarantees require service implementation and tests.
