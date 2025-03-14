namespace Itmo.TinyStorage.Application.Swagger.OpenApiSecurity.OAuth2;

public static class SwaggerGenOptionsExtensions
{
    private const string OAuth2Scheme = "OAuth2";

    public static void AddOAuth2Security(this SwaggerGenOptions options, OpenApiOAuthFlow flow) =>
        options
            .AddOAuth2SecurityDefinition(flow)
            .AddOAuth2SecurityRequirement();

    private static SwaggerGenOptions AddOAuth2SecurityDefinition(this SwaggerGenOptions options, OpenApiOAuthFlow flow)
    {
        var openApiSecurityScheme = new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.OAuth2,
            Name = "Authorization",
            Description = OAuth2Scheme,
            Flows = new OpenApiOAuthFlows { AuthorizationCode = flow }
        };

        options.AddSecurityDefinition(OAuth2Scheme, openApiSecurityScheme);
        return options;
    }

    private static SwaggerGenOptions AddOAuth2SecurityRequirement(this SwaggerGenOptions options)
    {
        var openApiSecurityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = OAuth2Scheme }
                },
                new List<string>()
            }
        };

        options.AddSecurityRequirement(openApiSecurityRequirement);
        return options;
    }
}
