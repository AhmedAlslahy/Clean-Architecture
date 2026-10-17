using Application.Common.Interfaces;
using Domain.Entities.Identity;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Services;

public class UserService(SarhneDbContext context) : IUserService
{
    public async Task<User?> FindByEmailAsync(string email)
    {
        var normalizedEmail = email.ToUpperInvariant();

        return await context.Set<User>()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
    }

    public async Task<User?> FindByIdAsync(int userId)
    {
        return await context.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IList<string>> GetRolesAsync(int userId)
    {
        return await context.Set<User>()
            .Where(u => u.Id == userId)
            .SelectMany(u => u.UserRoles)
            .Select(ur => ur.Role.Name)
            .ToListAsync();
    }

    public async Task<bool> AddRoleToUserAsync(int userId, string roleName)
    {
        var user = await context.Set<User>()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return false;

        var role = await context.Set<Role>()
            .FirstOrDefaultAsync(r => r.Name == roleName);

        if (role is null)
            return false;

        if (user.UserRoles.Any(ur => ur.RoleId == role.Id))
            return false;

        user.UserRoles.Add(new UserRole
        {
            UserId = userId,
            RoleId = role.Id
        });

        await context.SaveChangesAsync();
        return true;
    }

    public async Task AddRoleAsync(User user, string roleName)
    {
        var role = await context.Set<Role>()
            .FirstOrDefaultAsync(r => r.Name == roleName)
            ?? throw new InvalidOperationException("Role not found.");

        var userRoleExists = await context.Set<UserRole>()
            .AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);

        if (userRoleExists)
            return;

        context.Set<UserRole>().Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        await context.SaveChangesAsync();
    }
}