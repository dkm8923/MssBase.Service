using Microsoft.AspNetCore.Authorization;

namespace MssBase.Service.Shared.Authorization;

public sealed class RequiredPermissionAttribute : AuthorizeAttribute
{
    private const string PolicyPrefix = "Permission:";

    public RequiredPermissionAttribute(string permission)
    {
        Policy = $"{PolicyPrefix}{permission}";
    }
}