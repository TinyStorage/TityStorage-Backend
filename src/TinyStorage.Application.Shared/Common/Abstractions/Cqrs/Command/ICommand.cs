namespace Itmo.TinyStorage.Application.Shared.Common.Abstractions.Cqrs.Command;

#pragma warning disable CA1040
public interface ICommand : IRequest
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
#pragma warning restore CA1040
