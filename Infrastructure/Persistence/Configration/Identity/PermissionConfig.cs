using Domain.Entities.Identity;

namespace Infrastructure.Persistence.Configration.Identity;

public class PermissionConfig : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}