﻿namespace Itmo.TinyStorage.WebAPI.V1.ItemAudits.Responses;

public sealed record GetItemAuditsResponse(IReadOnlyCollection<ItemAuditView> Data);