using Application.Common.Abstraction;
using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Extensions;

public static class UserExtensions
{
    public static async Task<User?> FindByEmailAsync(
        this IQueryable<User> users, string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToUpperInvariant();

        return await users.FirstOrDefaultAsync(
            u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public static async Task<User?> FindByUserNameAsync(
        this IQueryable<User> users, string userName,
        CancellationToken cancellationToken = default)
    {
        var normalizedUserName = userName.Trim().ToUpperInvariant();

        return await users.FirstOrDefaultAsync(
            u => u.NormalizedUserName == normalizedUserName, cancellationToken);
    }

    public static async Task<User?> FindByIdAsync(
        this IQueryable<User> users, int userId,
        CancellationToken cancellationToken = default)
    {
        return await users.FirstOrDefaultAsync(
            u => u.Id == userId, cancellationToken);
    }

    public static async Task<IList<string>> GetRolesAsync(
        this IQueryable<User> users, int userId,
        CancellationToken cancellationToken = default)
    {
        return await users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.UserRoles)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);
    }

    public static async Task<bool> AddRoleToUserAsync(
        this IApplicationDbContext context,
        int userId, string roleName,
        CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(
                u => u.Id == userId,
                cancellationToken);

        if (user is null)
            return false;

        var normalizedRoleName = roleName.Trim().ToUpperInvariant();

        var role = await context.Roles
            .FirstOrDefaultAsync(
                r => r.NormalizedName == normalizedRoleName, cancellationToken);

        if (role is null)
            return false;

        if (user.UserRoles.Any(ur => ur.RoleId == role.Id))
            return false;

        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public static async Task AddRoleAsync(this IApplicationDbContext context,
        User user, string roleName,
        CancellationToken cancellationToken = default)
    {
        var normalizedRoleName = roleName.Trim().ToUpperInvariant();

        var role = await context.Roles
            .FirstOrDefaultAsync(
                r => r.NormalizedName == normalizedRoleName, cancellationToken)
            ?? throw new InvalidOperationException("Role not found.");

        var userRoleExists = await context.UserRoles
            .AnyAsync(ur => ur.UserId == user.Id
                && ur.RoleId == role.Id, cancellationToken);

        if (userRoleExists)
            return;

        context.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}