namespace API.Models;

public sealed class SendMessageRequest
{
    public string? Content { get; set; }
    public IFormFile? Photo { get; set; }
    public int ReceiverId { get; set; }
}