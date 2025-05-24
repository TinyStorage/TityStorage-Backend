using System.Text.Json;

namespace Itmo.TinyStorage.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddTinyStorageAuth(this IServiceCollection services)
    {
        services
            .ConfigureOptions<ConfigureJwtAuthOptions>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is ClaimsIdentity identity)
                        {
                            var roleClaims = context.Principal.FindFirst("realm_access")?.Value;
                            if (!string.IsNullOrEmpty(roleClaims))
                            {
                                using var doc = JsonDocument.Parse(roleClaims);
                                if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
                                {
                                    foreach (var role in rolesElement.EnumerateArray())
                                    {
                                        identity.AddClaim(new Claim(ClaimTypes.Role, role.GetString() ?? string.Empty));
                                    }
                                }
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services
            .AddHttpContextAccessor()
            .AddScoped<IUserAccessor, UserAccessor>();

        return services;
    }
}
