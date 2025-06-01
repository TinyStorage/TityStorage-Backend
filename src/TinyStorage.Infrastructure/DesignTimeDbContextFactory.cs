namespace Itmo.TinyStorage.Infrastructure;

internal sealed class DesignTimeDbContextFactory(IUserAccessor user) : IDesignTimeDbContextFactory<TinyStorageContext>
{
    public TinyStorageContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseNpgsql(new NpgsqlConnection())
            .Options;

        return new TinyStorageContext(options, user);
    }
}
