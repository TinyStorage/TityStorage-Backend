namespace Itmo.TinyStorage.Application.ExceptionHandling;

public sealed class ExceptionHandlingOptions
{
    internal IDictionary<Type, Func<Exception, HttpContext, ExceptionHandlingResult>> Handlers { get; } =
        new Dictionary<Type, Func<Exception, HttpContext, ExceptionHandlingResult>>();

    public ExceptionHandlingMode Mode { get; set; } = ExceptionHandlingMode.Strict;

    public ExceptionHandlingOptions MapException<TException>(
        Func<Exception, HttpContext, ExceptionHandlingResult> handler)
    {
        Handlers[typeof(TException)] = handler ?? throw new ArgumentNullException(nameof(handler));
        return this;
    }
}