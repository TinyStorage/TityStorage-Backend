namespace Itmo.TinyStorage.Application.ExceptionHandling;

public class ExceptionHandlingResult
{
    public int StatusCode { get; set; } = StatusCodes.Status500InternalServerError;
    public LogLevel LogLevel { get; set; } = LogLevel.Error;
    public string ErrorCode { get; set; } = "INTERNAL_ERROR";
    public string ErrorMessage { get; set; } = "Something went wrong. Please try again later.";
    public object? ErrorDetails { get; set; }
}