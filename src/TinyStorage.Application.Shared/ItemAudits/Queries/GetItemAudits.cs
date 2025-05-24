namespace Itmo.TinyStorage.Application.Shared.ItemAudits.Queries;

public sealed record ItemAuditView(Guid Id, string ItemName, string Property, string Value);

public sealed record GetItemAuditsQuery() : IQuery<IReadOnlyCollection<ItemAuditView>>;

[UsedImplicitly]
public sealed class GetItemsAuthorizer(ILogger<GetItemsAuthorizer> logger, IUserAccessor user)
    : IAuthorizer<GetItemAuditsQuery>
{
    public async Task<AuthorizationResult> AuthorizeAsync(GetItemAuditsQuery @query, CancellationToken cancellation)
    {
        if (!user.IsAdministrator)
        {
            logger.LogInformation("User {Isu} has not role {Role}", user.Isu, "Администратор");

            return await Task.FromResult(AuthorizationResult.Failed());
        }

        return await Task.FromResult(AuthorizationResult.Success());
    }
}

public sealed class GetItemAuditsHandler(ILogger<GetItemAuditsHandler> logger, TinyStorageContext context) : IQueryHandler<GetItemAuditsQuery, IReadOnlyCollection<ItemAuditView>>
{
    private readonly DbSet<ItemAuditModel> _item = context.ItemAudits;
    
    public async Task<IReadOnlyCollection<ItemAuditView>> Handle(GetItemAuditsQuery request,
        CancellationToken cancellationToken)
    {
        var itemAudits = await _item 
            .AsNoTracking()
            .AsQueryable()
            .Select(itemAudit => new ItemAuditView(itemAudit.Id, itemAudit.ItemModel!.Name, itemAudit.Property, itemAudit.Value))
            .ToArrayAsync(cancellationToken);
        
        logger.LogInformation("Get {Count} item audits", itemAudits.Length);

        return itemAudits;
    }
}