namespace Itmo.TinyStorage.Application.Shared.Common.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(
    IServiceProvider serviceProvider,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = default(TResponse);
        var typeName = request.GetGenericTypeName();

        try
        {
            if (request is IQuery or IQuery<TResponse>)
            {
                return await next();
            }

            var dbContext = serviceProvider.GetRequiredService<TinyStorageContext>();

            if (dbContext.HasActiveTransaction)
            {
                return await next();
            }

            return await dbContext.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                Guid transactionId;

                await using var transaction = await dbContext.BeginTransactionAsync(cancellationToken);

                using (logger.BeginScope(new List<KeyValuePair<string, object>>
                       {
                           new("TransactionContext", transaction!.TransactionId)
                       }))
                {
                    logger.LogInformation("Begin transaction {TransactionId} for {CommandName} ({@Command})",
                        transaction.TransactionId, typeName, request);

                    response = await next();

                    logger.LogInformation("Commit transaction {TransactionId} for {CommandName}",
                        transaction.TransactionId, typeName);

                    await dbContext.CommitTransactionAsync(transaction, cancellationToken);

                    transactionId = transaction.TransactionId;
                }

                return response;
            });
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error Handling transaction for {CommandName} ({@Command})", typeName, request);

            throw;
        }
    }
}
