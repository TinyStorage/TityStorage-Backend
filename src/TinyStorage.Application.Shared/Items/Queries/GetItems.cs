namespace Itmo.TinyStorage.Application.Shared.Items.Queries;

public sealed record ItemView(Guid Id, string Name, int? TakenBy);

public sealed record GetItemsQuery() : IQuery<IReadOnlyCollection<ItemView>>;

[UsedImplicitly]
public sealed class GetItemsAuthorizer(ILogger<GetItemsAuthorizer> logger, IUserAccessor user)
    : IAuthorizer<GetItemsQuery>
{
    public async Task<AuthorizationResult> AuthorizeAsync(GetItemsQuery @query, CancellationToken cancellation)
    {
        if (!user.IsLaboratoryAssistant)
        {
            logger.LogInformation("User {Isu} has not role {Role}", user.Isu, "Лаборант");

            return await Task.FromResult(AuthorizationResult.Failed());
        }

        return await Task.FromResult(AuthorizationResult.Success());
    }
}

public sealed class GetItemsQueryHandler(
    ILogger<GetItemsQueryHandler> logger,
    TinyStorageContext context) : IQueryHandler<GetItemsQuery, IReadOnlyCollection<ItemView>>
{
    private readonly DbSet<ItemModel> _item = context.Items;

    public async Task<IReadOnlyCollection<ItemView>> Handle(GetItemsQuery query, CancellationToken cancellationToken)
    {
        var items = await _item
            .AsNoTracking()
            .AsQueryable()
            .Select(item => new ItemView(item.Id, item.Name, item.TakenBy))
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Get {Count} items", items.Length);

        return items;
    }
}