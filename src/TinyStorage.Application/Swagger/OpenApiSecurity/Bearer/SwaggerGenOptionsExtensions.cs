namespace Itmo.TinyStorage.Application.Swagger.OpenApiSecurity.Bearer;

public static class SwaggerGenOptionsExtensions
{
    private const string BearerScheme = "Bearer";

    public static void AddJwtBearerSecurity(this SwaggerGenOptions options) =>
        options
            .AddJwtBearerSecurityDefinition()
            .AddJwtBearerSecurityRequirement();

    private static SwaggerGenOptions AddJwtBearerSecurityDefinition(this SwaggerGenOptions options)
    {
        var openApiSecurityScheme = new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Name = "Authorization",
            Description = "Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
            BearerFormat = "JWT",
            Scheme = BearerScheme
        };

        options.AddSecurityDefinition(BearerScheme, openApiSecurityScheme);
        return options;
    }

    private static SwaggerGenOptions AddJwtBearerSecurityRequirement(this SwaggerGenOptions options)
    {
        var openApiSecurityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = BearerScheme }
                },
                new List<string>()
            }
        };

        options.AddSecurityRequirement(openApiSecurityRequirement);
        return options;
    }
}
