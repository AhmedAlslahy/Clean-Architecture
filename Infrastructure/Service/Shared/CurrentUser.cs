using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Service.Shared;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string? UserId =>
        httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
}