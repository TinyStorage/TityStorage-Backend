namespace Itmo.TinyStorage.WebAPI.V1;

public static class DependencyInjection
{
    private const int MajorVersion = 1;
    private const int MinorVersion = 0;

    public static IServiceCollection AddTinyStorageWebApiV1Controllers(this IServiceCollection service)
    {
        service.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                typeof(DependencyInjection).Assembly
                    .GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(ControllerBase)))
                    .ToList()
                    .ForEach(controller => options.Conventions
                        .Controller(controller)
                        .HasApiVersion(MajorVersion, MinorVersion));
            })
            .AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return service;
    }
}
