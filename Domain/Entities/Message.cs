using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities;

public class Message : BaseEntity
{
    public string? Content { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsRead { get; private set; } = false;
    public bool IsStarred { get; private set; } = false;

    //Relations
    public int ReceiverId { get; set; }

    public User Receiver { get; set; } = null!;
    public int SenderId { get; set; }
    public User Sender { get; set; } = null!;

    //------------------------------------------------------------------
    private Message() { }

    public Message(int senderId, int receiverId, string? content, string? photoUrl)
    {
        if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(photoUrl))
            throw new ArgumentException("Message must contain content or a photo.");

        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        PhotoUrl = photoUrl;
        IsRead = false;
        IsStarred = false;
    }

    public void MarkAsRead()
    {
        if (IsRead)
            return;

        IsRead = true;
    }

    public void ToggleStar()
    {
        IsStarred = !IsStarred;
    }
}