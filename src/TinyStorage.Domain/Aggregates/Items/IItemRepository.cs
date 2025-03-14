namespace Itmo.TinyStorage.Domain.Aggregates.Items;

public interface IItemRepository
{
    Task<Item?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
}