using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Shared.Logic.Common;

namespace MssBase.Service.Shared.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

        foreach (var claim in context.User.FindAll(Constants.PermissionsClaim))
        {
            if (string.Equals(claim.Value, requirement.Permission, StringComparison.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            try
            {
                var permissions = JsonSerializer.Deserialize<List<string>>(claim.Value);

                if (permissions?.Any(permission => string.Equals(permission, requirement.Permission, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            catch
            {
                // Ignore malformed permission values and continue checking.
            }
        }

        return Task.CompletedTask;
    }
}