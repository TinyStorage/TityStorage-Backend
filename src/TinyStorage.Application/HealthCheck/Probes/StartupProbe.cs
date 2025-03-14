namespace Itmo.TinyStorage.Application.HealthCheck.Probes;

public sealed class StartupProbe(IHostApplicationLifetime lifetime) : IHealthCheck
{
    private readonly bool _started = lifetime.ApplicationStarted.IsCancellationRequested;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken)
    {
        var healthCheckResult = _started ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();

        return await Task.FromResult(healthCheckResult);
    }
}