namespace Itmo.TinyStorage.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTinyStorageInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<TinyStorageContext>((provider, builder) =>
        {
            var infrastructureSettings = provider.GetRequiredService<IOptions<InfrastructureSettings>>().Value;
            builder
                .UseNpgsql(infrastructureSettings.ConnectionString)
                .UseSnakeCaseNamingConvention()
                .EnableSensitiveDataLogging();
        });

        return services;
    }

    public static async Task ApplyTinyStorageMigrationsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TinyStorageContext>();
        await dbContext.Database.MigrateAsync();
    }
}
