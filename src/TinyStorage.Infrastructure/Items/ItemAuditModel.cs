namespace Itmo.TinyStorage.Infrastructure.Items;

public sealed class ItemAuditModel
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public int EditedBy { get; set; }
    public required string Property { get; set; }
    public required string Value { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public ItemModel? ItemModel { get; set; }
}