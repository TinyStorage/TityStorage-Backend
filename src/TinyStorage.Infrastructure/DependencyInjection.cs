namespace Itmo.TinyStorage.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTinyStorageInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<TinyStorageContext>()
            .ConfigureOptions<DbContextConfigureOptions>();

        return services;
    }

    public static async Task ApplyTinyStorageMigrationsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TinyStorageContext>();
        await dbContext.Database.MigrateAsync();
    }
}