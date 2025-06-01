namespace Itmo.TinyStorage.Infrastructure.Items;

public sealed class UpdateAuditableInterceptor(IUserAccessor user) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await CreateAuditableEntitiesAsync(eventData.Context, cancellationToken)
                .ConfigureAwait(false);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task CreateAuditableEntitiesAsync(DbContext context, CancellationToken cancellationToken)
    {
        var entityEntryCollection = context.ChangeTracker
            .Entries<ItemModel>()
            .Where(entityEntry => entityEntry.State == EntityState.Modified);
        foreach (var entityEntry in entityEntryCollection)
        {
            await AddAuditEntryAsync(context, entityEntry, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private async Task AddAuditEntryAsync(DbContext context,
        EntityEntry<ItemModel> entityEntry,
        CancellationToken cancellationToken)
    {
        var audit = new ItemAuditModel
        {
            Id = Guid.NewGuid(),
            ItemId = entityEntry.Entity.Id,
            EditedBy = user.Isu,
            Property = GetChangedProperty(entityEntry),
            Value = GetChangedPropertyValue(entityEntry),
            CreatedAt = DateTime.UtcNow
        };

        await context.Set<ItemAuditModel>()
            .AddAsync(audit, cancellationToken)
            .ConfigureAwait(false);
    }

    private string GetChangedProperty(EntityEntry<ItemModel> entityEntry)
    {
        var changedProperty = entityEntry.Properties
            .FirstOrDefault(p => p.IsModified);

        return changedProperty?.Metadata.Name ?? string.Empty;
    }

    private string GetChangedPropertyValue(EntityEntry<ItemModel> entityEntry)
    {
        var changedProperty = entityEntry.Properties
            .FirstOrDefault(p => p.IsModified);

        var value = changedProperty?.CurrentValue;
        return value?.ToString() ?? string.Empty;
    }
}
