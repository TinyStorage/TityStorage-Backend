namespace Itmo.TinyStorage.Auth.Settings;

public sealed class JwtAuthSettings
{
    public string? Authority { get; set; }
    public string? Audience { get; set; }
    public bool RequireHttpsMetadata { get; set; }
    public TokenValidationParameters TokenValidationParameters { get; set; } = new();
}
