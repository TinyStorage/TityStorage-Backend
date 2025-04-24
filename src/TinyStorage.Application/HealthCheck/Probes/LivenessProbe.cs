namespace Itmo.TinyStorage.Application.HealthCheck.Probes;

public sealed class LivenessProbe(IHostApplicationLifetime lifetime) : IHealthCheck
{
    private readonly bool _healthy = lifetime.ApplicationStarted.IsCancellationRequested;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken)
    {
        var healthCheckResult = _healthy ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();

        return await Task.FromResult(healthCheckResult);
    }
}
