namespace Itmo.TinyStorage.Application.Shared.ItemAudits.Queries;

public sealed record ItemAuditView;

public sealed record GetItemAudits(int? Offset, int? Limit) : IQuery<IReadOnlyCollection<ItemAuditView>>;

public sealed class GetItemAuditsHandler : IQueryHandler<GetItemAudits, IReadOnlyCollection<ItemAuditView>>
{
    public async Task<IReadOnlyCollection<ItemAuditView>> Handle(GetItemAudits request,
        CancellationToken cancellationToken) =>
        new List<ItemAuditView>().ToArray();
}
