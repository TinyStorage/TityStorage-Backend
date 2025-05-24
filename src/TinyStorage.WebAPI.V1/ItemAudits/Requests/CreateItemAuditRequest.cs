namespace Itmo.TinyStorage.WebAPI.V1.ItemAudits.Requests;

public sealed record CreateItemAuditRequest(Guid Id, Guid ItemId, int EditedBy, string Property, string Value);