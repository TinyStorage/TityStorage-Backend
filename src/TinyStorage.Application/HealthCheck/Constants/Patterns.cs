namespace Itmo.TinyStorage.Application.HealthCheck.Constants;

public static class Patterns
{
    public const string Liveness = "/healthz";
    public const string Readiness = "/readyz";
    public const string Startup = "/startupz";
}
