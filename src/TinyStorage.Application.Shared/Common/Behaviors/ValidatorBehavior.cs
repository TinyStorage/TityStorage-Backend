using Itmo.TinyStorage.Application.Shared.Common.Exceptions;

namespace Itmo.TinyStorage.Application.Shared.Common.Behaviors;

public sealed class ValidatorBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var typeName = request.GetGenericTypeName();

        logger.LogInformation("Validating command {CommandType}", typeName);

        var failures = validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(error => error != null);

        if (failures.Any())
        {
            logger.LogError("Validation errors - {CommandType} - Command: {Command} - Errors: {ValidationErrors}",
                typeName,
                request,
                failures);

            if (request is IQuery or IQuery<TResponse>)
            {
                return default!;
            }
            
            throw new ValidationDomainException(
                $"Command Validation Errors for type {typeof(TRequest).Name}",
                new ValidationException("Validation exception", failures));
        }

        return await next();
    }
}
