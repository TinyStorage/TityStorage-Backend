namespace Itmo.TinyStorage.Domain.Aggregates.Items;

public sealed class Item
{
    private bool _isTaken;
    private int? _takenBy;

    public Item(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Item(Guid id, string name, int? takenBy)
    {
        Id = id;
        Name = name;
        TakenBy = takenBy;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public int? TakenBy
    {
        get => _takenBy;
        private set
        {
            _takenBy = value;
            _isTaken = value.HasValue;
        }
    }

    public void Take(int isu)
    {
        if (_isTaken)
        {
            throw new ItemDomainException($"Item is already taken by {_takenBy}");
        }

        TakenBy = isu;
    }

    public void Give(int isu)
    {
        if (!_isTaken)
        {
            throw new ItemDomainException($"Item is not taken");
        }

        if (TakenBy != isu)
        {
            throw new ItemDomainException($"Item is taken by {_takenBy}");
        }

        TakenBy = null;
    }
}
