namespace Domain.Entities.Identity;

public class UserRole
{
    public int UserId { get; set; } = default!;
    public User User { get; set; } = default!;

    public int RoleId { get; set; } = default!;
    public Role Role { get; set; } = default!;
}