namespace Application.Common.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; }
    public bool IsAuthenticated { get; }
}