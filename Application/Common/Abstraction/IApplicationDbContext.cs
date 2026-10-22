using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Entities.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Abstraction;

public interface IApplicationDbContext
{
    //Identity
    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    //Entities
    public DbSet<UserSetting> UserSettings { get; set; }

    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Message> Messages { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}