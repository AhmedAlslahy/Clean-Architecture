using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Entities.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Abstraction;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Role> Roles { get; set; }
    DbSet<UserRole> UserRoles { get; set; }
    DbSet<UserSetting> UserSettings { get; set; }
    DbSet<Notification> Notifications { get; set; }
    DbSet<Message> Messages { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }
    DbSet<OutboxMessage> OutboxMessages { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}