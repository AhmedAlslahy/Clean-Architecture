using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = true)]
public sealed class HasPermissionAttribute(string permission)
    : AuthorizeAttribute(policy: permission);