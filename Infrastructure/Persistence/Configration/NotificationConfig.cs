namespace Infrastructure.Persistence.Configration;

public class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}