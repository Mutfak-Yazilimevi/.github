namespace LeatherErp.Api.Auth;

/// <summary>appsettings'ten okunan JWT yapılandırması.</summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "LeatherErp";
    public string Audience { get; set; } = "LeatherErpClients";
    public int ExpiryMinutes { get; set; } = 480;
}
