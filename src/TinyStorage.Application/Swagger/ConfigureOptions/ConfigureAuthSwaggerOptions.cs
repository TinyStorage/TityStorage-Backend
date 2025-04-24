namespace Itmo.TinyStorage.Application.Swagger.ConfigureOptions;

public sealed class ConfigureAuthSwaggerOptions(IOptions<OpenApiOAuthFlow> options)
    : IConfigureOptions<SwaggerGenOptions>
{
    private readonly OpenApiOAuthFlow _openApiOAuthFlow = options.Value;

    public void Configure(SwaggerGenOptions options)
    {
        options.AddOAuth2Security(_openApiOAuthFlow);
        options.AddJwtBearerSecurity();
    }
}
