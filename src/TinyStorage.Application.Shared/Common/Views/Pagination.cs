namespace Itmo.TinyStorage.Application.Shared.Common.Views;

public sealed record Pagination(int? Offset, int? Limit, int? Total);