using Itmo.TinyStorage.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Itmo.TinyStorage.Infrastructure.ConfigureOptions;

public sealed class DbContextConfigureOptions(IOptions<InfrastructureSettings> infrastructureSettingsOptions)
    : IConfigureNamedOptions<DbContextOptionsBuilder>
{
    public void Configure(DbContextOptionsBuilder options) => Configure(nameof(DbContextOptionsBuilder), options);

    public void Configure(string? name, DbContextOptionsBuilder options) =>
        options
            .UseNpgsql(infrastructureSettingsOptions.Value.ConnectionString)
            .UseSnakeCaseNamingConvention()
            .EnableSensitiveDataLogging();
}