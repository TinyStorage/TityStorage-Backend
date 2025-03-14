namespace Itmo.TinyStorage.Application.Shared.Common.Exceptions;

public sealed class ValidationDomainException(string message, Exception innerException)
    : DomainException(message, innerException);