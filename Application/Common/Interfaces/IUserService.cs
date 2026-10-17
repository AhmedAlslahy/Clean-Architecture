using Domain.Entities.Identity;

namespace Application.Common.Interfaces;

public interface IUserService
{
    Task<User?> FindByEmailAsync(string email);

    Task<User?> FindByIdAsync(int userId);

    Task<IList<string>> GetRolesAsync(int userId);

    Task<bool> AddRoleToUserAsync(int userId, string roleName);

    Task AddRoleAsync(User user, string roleName);
}