namespace Itmo.TinyStorage.Application.Shared.Common.Abstractions.Cqrs.Query;

#pragma warning disable CA1040
public interface IQuery : IRequest { }

public interface IQuery<out TResponse> : IRequest<TResponse> { }
#pragma warning restore CA1040
