using Domain.Identity;

namespace Infrastructure.Service.JWT;

public interface IJwtService
{
    Task<Result<GenerateTokenResDto>> GenerateToken(User user, IList<string> roles);
}