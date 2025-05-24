namespace Itmo.TinyStorage.Infrastructure.Items;

public sealed class ItemModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int? TakenBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public IEnumerable<ItemAuditModel>? ItemAuditModels { get; set; }
}
