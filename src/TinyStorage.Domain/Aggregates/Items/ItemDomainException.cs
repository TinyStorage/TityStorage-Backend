namespace Itmo.TinyStorage.Domain.Aggregates.Items;

public sealed class ItemDomainException(string? message) : DomainException(message);
