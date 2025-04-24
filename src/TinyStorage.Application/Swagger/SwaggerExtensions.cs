namespace Itmo.TinyStorage.Application.Swagger;

public static class SwaggerExtensions
{
    private const string Name = "TinyStorage API";

    public static IServiceCollection AddSwagger(this IServiceCollection services, string serverUrl)
    {
        services
            .ConfigureOptions<ConfigureAuthSwaggerOptions>()
            .AddSwaggerGen(options =>
            {
                options.AddServer(new OpenApiServer { Description = Name, Url = serverUrl });
                var provider = services
                    .BuildServiceProvider()
                    .GetRequiredService<IApiDescriptionGroupCollectionProvider>();

                var groupNameCollection = provider.ApiDescriptionGroups.Items
                    .Where(apiDescriptionGroup => apiDescriptionGroup.GroupName is not null)
                    .Select(apiDescriptionGroup => apiDescriptionGroup.GroupName);

                foreach (var groupName in groupNameCollection)
                {
                    options.SwaggerDoc(groupName, new OpenApiInfo { Title = Name, Version = groupName });
                    options.DocInclusionPredicate((name, apiDescription)
                        => apiDescription.GroupName!.Contains(name, StringComparison.Ordinal));
                }

                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                options.CustomSchemaIds(x => x.FullName);

                Directory
                    .GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly)
                    .ToList()
                    .ForEach(xmlFile => options.IncludeXmlComments(xmlFile));
            });

        return services;
    }

    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, string applicationBasePath)
    {
        app.UseSwagger(swaggerOptions =>
        {
            swaggerOptions.RouteTemplate = "/swagger/{documentName}/swagger.json";
            swaggerOptions.PreSerializeFilters.Add((document, request) =>
                document.Servers = new List<OpenApiServer> { new() { Url = applicationBasePath } });
        });

        var apiDescriptionGroupCollectionProvider =
            app.ApplicationServices.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

        app.UseSwaggerUI(options =>
        {
            var apiDescriptionGroupCollection = apiDescriptionGroupCollectionProvider
                .ApiDescriptionGroups.Items
                .Where(apiDescriptionGroup => apiDescriptionGroup.GroupName is not null);

            foreach (var description in apiDescriptionGroupCollection)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
            }
        });

        return app;
    }
}
