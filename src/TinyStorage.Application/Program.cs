var builder = WebApplication.CreateBuilder(args);
var applicationBasePath = builder.Configuration.GetValue<string>("ApplicationBasePath") ??
                          throw new InvalidOperationException("Missing default ApplicationBasePath");

builder.ConfigureSettings();

builder.Services.AddTinyStorageHealthChecks();

builder.Services.AddTinyStorageAuth();
builder.Services.AddTinyStorageApplicationSharedLayer();
builder.Services.AddTinyStorageInfrastructure();

builder.Services.AddAuthorization();
builder.Services
    .AddControllers()
    .AddJsonOptions(_ => { })
    .ConfigureTinyStorage();

builder.Services.AddTinyStorageWebApiV1Controllers();

builder.Services.AddSwagger(applicationBasePath);
builder.Services.AddTinyStorageCors();

var app = builder.Build();

app.MapOnInternalPort(privateApp => privateApp
    .UseFilterExceptions()
    .UseForwardedHeaders()
    .UseSwagger(applicationBasePath)
    .UseRouting()
    .UseTinyStorageCors()
    .UseAuthentication()
    .UseAuthorization()
    .UseEndpoints(endpoints => endpoints.MapTinyStorageHealthChecks()));

app.MapOnPublicPort(publicApp => publicApp
    .UseFilterExceptions()
    .UseForwardedHeaders()
    .UseSwagger(applicationBasePath)
    .UseRouting()
    .UseTinyStorageCors()
    .UseAuthentication()
    .UseAuthorization()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapTinyStorageHealthChecks();
        endpoints.MapControllers();
    }));

await app.ApplyTinyStorageMigrationsAsync();

app.Run();