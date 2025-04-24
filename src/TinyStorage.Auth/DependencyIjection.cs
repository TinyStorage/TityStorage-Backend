namespace Itmo.TinyStorage.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddTinyStorageAuth(this IServiceCollection services)
    {
        services
            .ConfigureOptions<ConfigureJwtAuthOptions>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);

        services
            .AddHttpContextAccessor()
            .AddScoped<IUserAccessor, UserAccessor>();

        return services;
    }
}
