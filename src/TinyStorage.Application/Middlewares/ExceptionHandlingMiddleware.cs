namespace Itmo.TinyStorage.Application.Middlewares;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ExceptionHandlingOptions exceptionHandlingOptions,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly ILogger _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException exception) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogInformation(exception, "Request was cancelled");

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ErrorResponse
            {
                ErrorCode = "CANCELED", ErrorMessage = "Request was cancelled"
            });
        }
        catch (Exception exception)
        {
            LogLevel level;
            int httpStatusCode;
            ErrorResponse errorResponse;

            var handler = FindHandler(exception);

            if (handler != null)
            {
                var result = handler.Invoke(exception, context);
                level = result.LogLevel;
                httpStatusCode = result.StatusCode;
                errorResponse = new ErrorResponse
                {
                    ErrorCode = result.ErrorCode,
                    ErrorMessage = result.ErrorMessage,
                    ErrorDetails = result.ErrorDetails
                };
            }
            else
            {
                level = LogLevel.Error;
                httpStatusCode = StatusCodes.Status500InternalServerError;
                errorResponse = new ErrorResponse
                {
                    ErrorCode = "INTERNAL_ERROR", ErrorMessage = "Something went wrong. Please try again later."
                };
            }

            var logContext = new Dictionary<string, object?> { ["@Error"] = errorResponse };
            _logger.Log(level, 0, logContext, exception, (_, _) => exception.Message);

            context.Response.StatusCode = httpStatusCode;
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }

    private Func<Exception, HttpContext, ExceptionHandlingResult>? FindHandler(Exception exception)
    {
        var exceptionType = exception.GetType();
        Func<Exception, HttpContext, ExceptionHandlingResult>? handler = null;

        do
        {
            if (exceptionHandlingOptions.Handlers.TryGetValue(exceptionType, out var value))
            {
                handler = value;
            }
            else
            {
                exceptionType = exceptionType.BaseType;
            }
        } while (exceptionHandlingOptions.Mode == ExceptionHandlingMode.Hierarchical && handler == null &&
                 exceptionType != null);

        return handler;
    }
}
