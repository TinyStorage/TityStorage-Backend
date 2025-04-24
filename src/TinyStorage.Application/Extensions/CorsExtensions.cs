namespace Itmo.TinyStorage.Application.Extensions;

public static class CorsExtensions
{
    private const string CorsProductionPolicy = nameof(CorsProductionPolicy);
    private const string CorsDevelopmentPolicy = nameof(CorsDevelopmentPolicy);

    public static IServiceCollection AddTinyStorageCors(this IServiceCollection services)
    {
        services.AddCors(options => options
            .AddPolicy(CorsProductionPolicy, policy => policy
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()));

        services.AddCors(options => options
            .AddPolicy(CorsDevelopmentPolicy, policy => policy
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()));

        return services;
    }

    public static IApplicationBuilder UseTinyStorageCors(this IApplicationBuilder app)
    {
        var isAllowAllCorsEnabled = true;
        return app.UseCors(isAllowAllCorsEnabled ? CorsDevelopmentPolicy : CorsProductionPolicy);
    }
}
