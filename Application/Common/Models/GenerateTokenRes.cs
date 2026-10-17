namespace Application.Common.Models;

public class GenerateTokenRes
{
    public required string Token { get; set; }
    public DateTime ExpireIn { get; set; }
}