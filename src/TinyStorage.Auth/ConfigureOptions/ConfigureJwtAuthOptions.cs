namespace Itmo.TinyStorage.Auth.ConfigureOptions;

public sealed class ConfigureJwtAuthOptions(IOptions<JwtAuthSettings> options)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtAuthSettings _settings = options.Value;

    public void Configure(JwtBearerOptions options) => Configure(JwtBearerDefaults.AuthenticationScheme, options);

    public void Configure(string? name, JwtBearerOptions options)
    {
        options.Authority = _settings.Authority;
        options.RequireHttpsMetadata = _settings.RequireHttpsMetadata;
        options.TokenValidationParameters = _settings.TokenValidationParameters;
    }
}
