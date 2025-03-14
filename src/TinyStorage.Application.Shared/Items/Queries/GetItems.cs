namespace Itmo.TinyStorage.Application.Shared.Items.Queries;

public sealed record ItemView(Guid Id, string Name);

public sealed record GetItemsQuery : IQuery<IReadOnlyCollection<ItemView>>;

public sealed class GetItemsQueryHandler(ILogger<GetItemsQueryHandler> logger) : IQueryHandler<GetItemsQuery, IReadOnlyCollection<ItemView>>
{
    public async Task<IReadOnlyCollection<ItemView>> Handle(GetItemsQuery query, CancellationToken cancellationToken)
    {
        var items = new List<ItemView>().ToArray();

        return items;
    }
}