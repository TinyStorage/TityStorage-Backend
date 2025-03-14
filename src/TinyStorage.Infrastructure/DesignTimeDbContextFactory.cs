namespace Itmo.TinyStorage.Infrastructure;

internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TinyStorageContext>
{
    public TinyStorageContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseNpgsql(new NpgsqlConnection())
            .Options;

        return new TinyStorageContext(options);
    }
}
