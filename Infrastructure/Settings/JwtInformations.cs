namespace Infrastructure.Settings;

public class JwtInformations
{
    public string SecretKey { get; set; } = null!;
    public string IssuerIP { get; set; } = null!;
    public string AudienceIP { get; set; } = null!;
}