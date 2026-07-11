namespace Infrastructure.Service.JWT;

public class GenerateTokenResDto
{
    public required string Token { get; set; }
    public DateTime ExpireIn { get; set; }
}