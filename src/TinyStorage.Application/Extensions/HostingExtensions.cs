namespace Itmo.TinyStorage.Application.Extensions;

public static class HostingExtensions
{
    private static void MapOnPort(
        this IApplicationBuilder applicationBuilder,
        Action<IApplicationBuilder> configureAction,
        int port) => applicationBuilder.MapWhen(ctx => ctx.Connection.LocalPort == port, configureAction);

    public static void MapOnPublicPort(
        this IApplicationBuilder app,
        Action<IApplicationBuilder> configureAction,
        int defaultPort = 8080) => app.MapOnPort(configureAction, defaultPort);

    public static void MapOnInternalPort(
        this IApplicationBuilder app,
        Action<IApplicationBuilder> configureAction,
        int defaultPort = 8082) => app.MapOnPort(configureAction, defaultPort);
}