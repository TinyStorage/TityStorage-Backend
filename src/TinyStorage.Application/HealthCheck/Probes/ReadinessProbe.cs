namespace Itmo.TinyStorage.Application.HealthCheck.Probes;

public sealed class ReadinessProbe(IHostApplicationLifetime lifetime) : IHealthCheck
{
    private readonly bool _ready = lifetime.ApplicationStarted.IsCancellationRequested &&
                                   !lifetime.ApplicationStopping.IsCancellationRequested;

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken)
    {
        var healthCheckResult = _ready ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();

        return Task.FromResult(healthCheckResult);
    }
}