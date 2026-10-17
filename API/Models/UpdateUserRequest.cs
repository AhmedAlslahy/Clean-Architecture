namespace API.Models;

public sealed class UpdateUserRequest
{
    public required string FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfileDescription { get; set; }
    public string? PublicLink { get; set; }
    public IFormFile? Image { get; set; }
}