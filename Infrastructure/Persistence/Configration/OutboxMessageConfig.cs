using Domain.Entities.Outbox;

namespace Infrastructure.Persistence.Configration;

public class OutboxMessageConfig : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.Error)
            .HasMaxLength(2000);

        builder.Property(x => x.IsProcessed)
            .HasDefaultValue(false);

        builder.HasIndex(x => new
        {
            x.IsProcessed,
            x.OccurredTime
        });
    }
}