namespace Itmo.TinyStorage.Application.Shared.Common.Behaviors;

public sealed class AuthorizationBehavior<TRequest, TResponse>(
    IEnumerable<IAuthorizer<TRequest>> authorizers)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        foreach (var authorizer in authorizers)
        {
            var result = await authorizer.AuthorizeAsync(request, cancellationToken);
            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException();
            }
        }

        return await next();
    }
}