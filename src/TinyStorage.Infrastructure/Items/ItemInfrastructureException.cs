namespace Itmo.TinyStorage.Infrastructure.Items;

public sealed class ItemInfrastructureException : InfrastructureException
{
    public ItemInfrastructureException(string? message) : base(message)
    {
    }

    public ItemInfrastructureException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
