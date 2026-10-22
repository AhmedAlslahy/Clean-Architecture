using Application.Common.Abstraction;
using Domain.Enums;

namespace Application.Common.Errors;

public static class PermissionErrors
{
    public static Error NotFound
     = new Error("Permission.NotFound", "Permission not found", ErrorType.NotFound);
}