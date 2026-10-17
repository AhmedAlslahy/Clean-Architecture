using Application.Common.Abstraction;
using Application.Common.Models;
using Domain.Entities.Identity;

namespace Application.Common.Interfaces;

public interface IJwtService
{
    Task<Result<GenerateTokenRes>> GenerateToken(User user, IList<string> roles);
}