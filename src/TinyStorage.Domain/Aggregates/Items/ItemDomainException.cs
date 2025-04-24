namespace Itmo.TinyStorage.Domain.Aggregates.Items;

public sealed class ItemDomainException : DomainException
{
    public ItemDomainException(string? message) : base(message)
    {
    }

    public ItemDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
