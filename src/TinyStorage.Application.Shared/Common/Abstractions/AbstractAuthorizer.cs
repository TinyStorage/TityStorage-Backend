namespace Itmo.TinyStorage.Application.Shared.Common.Abstractions;

public interface IAuthorizer<T>
{
    Task<AuthorizationResult> AuthorizeAsync(T instance, CancellationToken cancellation = default);
}