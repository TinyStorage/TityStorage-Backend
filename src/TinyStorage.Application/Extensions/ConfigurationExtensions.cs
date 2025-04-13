namespace Itmo.TinyStorage.Application.Extensions;

public static class ConfigurationExtensions
{
    public static WebApplicationBuilder ConfigureSettings(this WebApplicationBuilder builder)
    {
        var configurationManager = builder.Configuration;

        builder.Services
            .Configure<OpenApiOAuthFlow>(configurationManager.GetSection(nameof(OpenApiOAuthFlow)))
            .Configure<JwtAuthSettings>(configurationManager.GetSection(nameof(JwtAuthSettings)))
            .Configure<InfrastructureSettings>(configurationManager.GetSection(nameof(InfrastructureSettings)));

        return builder;
    }
}