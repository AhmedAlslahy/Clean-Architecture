namespace Domain.Entities.Outbox;

public class OutboxMessage
{
    public int Id { get; private set; }
    public DateTime OccurredTime { get; private set; } = DateTime.UtcNow;
    public string Type { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;

    public DateTime? ProcessedTime { get; private set; }
    public bool IsProcessed { get; private set; }
    public string? Error { get; set; }

    public void MarkAsProcessed(DateTime time)
    {
        IsProcessed = true;
        ProcessedTime = time;
    }

    public OutboxMessage(string type, string content)
    {
        Type = type;
        Content = content;
    }

    private OutboxMessage()
    {
    }
}