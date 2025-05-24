namespace Itmo.TinyStorage.Application.HealthCheck;

public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddTinyStorageHealthChecks(this IServiceCollection services) =>
        services.AddHealthChecks()
            .AddCheck<StartupProbe>(nameof(StartupProbe), tags: Tags.Startup)
            .AddCheck<LivenessProbe>(nameof(LivenessProbe), tags: Tags.Liveness)
            .AddCheck<ReadinessProbe>(nameof(ReadinessProbe), tags: Tags.Readiness);

    public static void MapTinyStorageHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks(Patterns.Startup,
            new HealthCheckOptions { Predicate = registration => registration.Tags.IsSupersetOf(Tags.Startup) });

        endpoints.MapHealthChecks(Patterns.Liveness,
            new HealthCheckOptions { Predicate = registration => registration.Tags.IsSupersetOf(Tags.Liveness) });

        endpoints.MapHealthChecks(Patterns.Readiness,
            new HealthCheckOptions { Predicate = registration => registration.Tags.IsSupersetOf(Tags.Readiness) });
    }
}
