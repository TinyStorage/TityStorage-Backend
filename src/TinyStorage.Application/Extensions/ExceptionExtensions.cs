namespace Itmo.TinyStorage.Application.Extensions;

public static class ExceptionErrorCodes
{
    public const string OperationCanceled = "OPERATION_CANCELED_EXCEPTION_ERROR";
    public const string Domain = "DOMAIN_EXCEPTION_ERROR";
    public const string Infrastructure = "INFRASTRUCTURE_EXCEPTION_ERROR";
    public const string DbUpdate = "DB_UPDATE_EXCEPTION_ERROR";
    public const string UnsupportedMediaType = "UNSUPPORTED_MEDIA_TYPE_EXCEPTION_ERROR";
    public const string UnsupportedBodyFormat = "UNSUPPORTED_BODY_FORMAT_EXCEPTION_ERROR";
}

public static class ExceptionExtensions
{
    public static IApplicationBuilder UseFilterExceptions(this IApplicationBuilder app)
    {
        var options = new ExceptionHandlingOptions { Mode = ExceptionHandlingMode.Hierarchical };

        options.MapException<ValidationDomainException>((ex, ctx) =>
        {
            var (convertPropertyName, replacePropertyNames) = ctx.GetPropertyNameConverters();

            return new ExceptionHandlingResult
            {
                LogLevel = LogLevel.Warning,
                StatusCode = StatusCodes.Status400BadRequest,
                ErrorCode = ExceptionErrorCodes.Domain,
                ErrorMessage = "One or more validation errors occurred.",
                ErrorDetails = (ex.InnerException as ValidationException)?.Errors
                    .Where(f => f != null)
                    .GroupBy(f => f.PropertyName)
                    .ToDictionary(
                        grp => convertPropertyName(grp.Key),
                        grp => grp
                            .Select(validationFailure => validationFailure.ErrorMessage)
                            .Select(message => replacePropertyNames(message, grp.Key))
                            .ToArray())
            };
        });

        options.MapException<DomainException>((ex, _) => new ExceptionHandlingResult
        {
            LogLevel = LogLevel.Warning,
            StatusCode = StatusCodes.Status400BadRequest,
            ErrorCode = ExceptionErrorCodes.Domain,
            ErrorMessage = ex.Message
        });

        // options.MapException<InfrastructureException>((ex, _) => new ExceptionHandlingResult
        // {
        //     LogLevel = LogLevel.Warning,
        //     StatusCode = StatusCodes.Status404NotFound,
        //     ErrorCode = ExceptionErrorCodes.Infrastructure,
        //     ErrorMessage = ex.Message
        // });

        options.MapException<DbUpdateException>((ex, _) => new ExceptionHandlingResult
        {
            LogLevel = LogLevel.Warning,
            StatusCode = StatusCodes.Status409Conflict,
            ErrorCode = ExceptionErrorCodes.DbUpdate,
            ErrorMessage = ex.Message
        });

        options.MapException<UnsupportedMediaTypeException>((_, _) => new ExceptionHandlingResult
        {
            LogLevel = LogLevel.Warning,
            StatusCode = StatusCodes.Status415UnsupportedMediaType,
            ErrorCode = ExceptionErrorCodes.UnsupportedMediaType,
            ErrorMessage = "Unsupported media type"
        });

        options.MapException<UnsupportedBodyFormatException>((_, _) => new ExceptionHandlingResult
        {
            LogLevel = LogLevel.Warning,
            StatusCode = StatusCodes.Status415UnsupportedMediaType,
            ErrorCode = ExceptionErrorCodes.UnsupportedBodyFormat,
            ErrorMessage = "Unsupported body format"
        });

        options.MapException<OperationCanceledException>((ex, _) => new ExceptionHandlingResult
        {
            LogLevel = LogLevel.Warning,
            StatusCode = StatusCodes.Status499ClientClosedRequest,
            ErrorCode = ExceptionErrorCodes.OperationCanceled,
            ErrorMessage = ex.Message
        });

        return app.UseMiddleware<ExceptionHandlingMiddleware>(options);
    }

    public static IMvcBuilder ConfigureTinyStorage(this IMvcBuilder builder) =>
        builder
            .AddMvcOptions(options =>
            {
                var customInputFormatter = new CustomJsonInputFormatter(
                    options.InputFormatters.OfType<SystemTextJsonInputFormatter>().First()
                );

                options.InputFormatters.Clear();
                options.InputFormatters.Add(customInputFormatter);
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;

                var baseInvalidModelStateResponseFactory = options.InvalidModelStateResponseFactory;

                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    baseInvalidModelStateResponseFactory?.Invoke(actionContext);

                    var (convertPropertyName, replacePropertyNames) =
                        actionContext.HttpContext.GetPropertyNameConverters();

                    return new BadRequestObjectResult(new ErrorResponse
                    {
                        ErrorCode = "VALIDATION_ERROR",
                        ErrorMessage = "One or more validation errors occurred.",
                        ErrorDetails = actionContext.ModelState
                            .Where(modelError => modelError.Value!.Errors.Count > 0)
                            .ToDictionary(
                                modelError => convertPropertyName(modelError.Key),
                                modelError =>
                                    modelError.Value?
                                        .Errors
                                        .Select(e => e.ErrorMessage)
                                        .Select(m => replacePropertyNames(m, modelError.Key))
                                        .ToArray())
                    });
                };
            });
}

static file class HttpContextExtensions
{
    public static (Func<string, string> convertPropertyName, Func<string, string, string> replacePropertyNames)
        GetPropertyNameConverters(this HttpContext context)
    {
        var jsonOptions = context.RequestServices
            .GetService<IOptions<JsonOptions>>();

        Func<string, string> convertPropertyName = input => input;
        Func<string, string, string> replacePropertyNames = (input, _) => input;

        if (jsonOptions?.Value?.JsonSerializerOptions?.PropertyNamingPolicy is not null)
        {
            convertPropertyName = jsonOptions.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName;
            replacePropertyNames = (input, propertyName) =>
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    return string.Empty;
                }

                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    return input;
                }

                return input.Replace(propertyName, convertPropertyName(propertyName));
            };
        }

        return (convertPropertyName, replacePropertyNames);
    }
}

sealed file class UnsupportedMediaTypeException : Exception
{
}

sealed file class UnsupportedBodyFormatException : Exception
{
}

sealed file class CustomJsonInputFormatter : TextInputFormatter
{
    private readonly SystemTextJsonInputFormatter _jsonFormatter;

    public CustomJsonInputFormatter(SystemTextJsonInputFormatter jsonFormatter)
    {
        _jsonFormatter = jsonFormatter;
        SupportedMediaTypes.Add(MediaTypeNames.Application.Json);
        SupportedEncodings.Add(Encoding.UTF8);
    }

    protected override bool CanReadType(Type type) => true;
    public override bool CanRead(InputFormatterContext context) => true;

    public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context) =>
        ReadRequestBodyAsync(context, Encoding.UTF8);

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context,
        Encoding encoding)
    {
        if (!_jsonFormatter.CanRead(context))
        {
            throw new UnsupportedMediaTypeException();
        }

        var result = await _jsonFormatter.ReadRequestBodyAsync(context, encoding);
        if (!result.HasError)
        {
            return result;
        }

        const string bodyMarker = "$";
        var invalidJsonBody = context.ModelState
            .Any(modelError => modelError.Key == bodyMarker &&
                               modelError.Value!.Errors.Count > 0);

        if (invalidJsonBody)
        {
            throw new UnsupportedBodyFormatException();
        }

        return result;
    }
}

public enum ExceptionHandlingMode
{
    Strict,
    Hierarchical
}
