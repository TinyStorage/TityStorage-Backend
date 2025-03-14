namespace Itmo.TinyStorage.Application.Shared.Common.Abstractions.Cqrs.Query;

public interface IQueryHandler<in TRequest> : IRequestHandler<TRequest> where TRequest : IQuery { }

public interface IQueryHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IQuery<TResponse> { }
