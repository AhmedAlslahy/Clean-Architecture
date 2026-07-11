namespace Infrastructure.Service.Shared;

public interface ICurrentUser
{
    string? UserId { get; }
}