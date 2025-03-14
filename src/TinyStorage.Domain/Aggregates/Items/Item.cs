namespace Itmo.TinyStorage.Domain.Aggregates.Items;

public sealed class Item
{
    private readonly Guid _id;
    private readonly string _name;
    private readonly int _takenBy;
    private readonly bool _isTaken;

    public Item(Guid id, string name)
    {
        _id = id;
        _name = name;
    }
    
    public Guid Id => _id;
    public string Name => _name;
}