using Application.Common.Interfaces;
using Domain.Constants;
using Domain.Entities.Identity;

namespace Infrastructure.Persistence.Data.Seeding;

public static class SeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, IPasswordService passwordService)
    {
        var context = serviceProvider.GetRequiredService<SarhneDbContext>();

        // 1. Seed Roles
        if (!await context.Roles.AnyAsync(r => r.Name == "Admin"))
        {
            await context.Roles.AddAsync(new Role { Name = "Admin" });
        }

        if (!await context.Roles.AnyAsync(r => r.Name == "User"))
        {
            await context.Roles.AddAsync(new Role { Name = "User" });
        }

        await context.SaveChangesAsync();

        // 2. Seed Admin User
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

        if (!await context.Users.AnyAsync())
        {
            var admin = new User
            {
                FullName = "admin",
                UserName = "admin",
                Email = "admin@test.com",
                PasswordHashed = passwordService.HashPassword("Admin@123"),
                EmailConfirmed = true,
                UserSetting = new UserSetting(),
                UserRoles =
                {
                    new UserRole
                    {
                        RoleId = adminRole!.Id
                    }
                }
            };

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }

        // 3. Seed Permissions
        var existingPermissions = await context.Permissions
            .Select(p => p.Name)
            .ToListAsync();

        var newPermissions = Permissions.GetAll
            .Where(p => !existingPermissions.Contains(p))
            .Select(p => new Permission
            {
                Name = p,
                Group = p.Split(':')[1]
            })
            .ToList();

        if (newPermissions.Count != 0)
        {
            await context.Permissions.AddRangeAsync(newPermissions);
            await context.SaveChangesAsync();
        }

        if (adminRole is not null)
        {
            var allPermissionIds = await context.Permissions
                .Select(p => p.Id)
                .ToListAsync();

            var existingRolePermissionIds = await context.RolePermissions
                .Where(rp => rp.RoleId == adminRole.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var newRolePermissions = allPermissionIds
                .Where(pId => !existingRolePermissionIds.Contains(pId))
                .Select(pId => new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = pId
                }).ToList();

            if (newRolePermissions.Count != 0)
            {
                await context.RolePermissions.AddRangeAsync(newRolePermissions);
                await context.SaveChangesAsync();
            }
        }
    }
}