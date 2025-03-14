namespace Itmo.TinyStorage.Application.HealthCheck.Constants;

public static class Tags
{
    public static readonly string[] Startup = ["startup"];
    public static readonly string[] Liveness = ["liveness"];
    public static readonly string[] Readiness = ["readiness"];
}