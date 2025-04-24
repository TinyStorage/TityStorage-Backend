namespace Itmo.TinyStorage.WebAPI.V1.Items.Responses;

public sealed record GetItemsResponse(IReadOnlyCollection<ItemView> Data);
