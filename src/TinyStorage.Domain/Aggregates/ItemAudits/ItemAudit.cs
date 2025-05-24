namespace Itmo.TinyStorage.Domain.Aggregates.ItemAudits;

public sealed class ItemAudit
{
    private readonly Guid _id;
    private readonly Guid _itemId;
    private readonly int _editedBy;
    private readonly string _property;
    private readonly string _value;
    private readonly DateTime _createdAt;

    public ItemAudit(Guid id, Guid itemId, int editedBy, string property, string value)
    {
        _id = id;
        _itemId = itemId;
        _property = property;
        _value = value;
        _editedBy = editedBy;
    }

    public Guid Id => _id;
    public Guid ItemId => _itemId;
    public int EditedBy => _editedBy;
    public string Property => _property;
    public string Value => _value;
}