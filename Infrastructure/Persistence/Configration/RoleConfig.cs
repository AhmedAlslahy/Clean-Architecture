using Domain.Entities.Identity;

namespace Infrastructure.Persistence.Configration;

public class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}