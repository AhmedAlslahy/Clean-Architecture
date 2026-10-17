using Application.Common.Interfaces;
using Domain.Entities.Identity;

namespace Infrastructure.Persistence.Data.Seeding;

public static class SeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, IPasswordService passwordService)
    {
        var context = serviceProvider.GetRequiredService<SarhneDbContext>();

        // Seed Roles
        if (!await context.Roles.AnyAsync(r => r.Name == "Admin"))
        {
            await context.Roles.AddAsync(new Role
            {
                Name = "Admin"
            });
        }

        if (!await context.Roles.AnyAsync(r => r.Name == "User"))
        {
            await context.Roles.AddAsync(new Role
            {
                Name = "User"
            });
        }
        await context.SaveChangesAsync();

        // Seed Admin User
        if (!await context.Users.AnyAsync())
        {
            var role = await context.Set<Role>()
               .FirstOrDefaultAsync(r => r.Name == "Admin");
            var admin = new User
            {
                FullName = "admin",
                UserName = "admin",
                Email = "admin@test.com",
                PasswordHashed = passwordService.HashPassword("Admin@123"),
                EmailConfirmed = true,
                UserSetting = new UserSetting(),
                UserRoles = {
                     new UserRole{
                          RoleId = role!.Id
                       }
                }
            };
            var result = await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}