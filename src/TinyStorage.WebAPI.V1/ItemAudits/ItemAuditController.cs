using Itmo.TinyStorage.Application.Shared.ItemAudits.Commands;
using Itmo.TinyStorage.Application.Shared.ItemAudits.Queries;
using Itmo.TinyStorage.WebAPI.V1.ItemAudits.Requests;
using Itmo.TinyStorage.WebAPI.V1.ItemAudits.Responses;

namespace Itmo.TinyStorage.WebAPI.V1.ItemAudits;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = ApiGroupNames.Name)]
[Route("v{version:apiVersion}/item-audits")]
public class ItemAuditController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateItemAuditAsync([FromBody] CreateItemAuditRequest request)
    {
        var itemAuditId = await mediator
            .Send(new CreateItemAuditCommand(request.Id, request.ItemId, request.EditedBy, request.Property, request.Value), HttpContext.RequestAborted)
            .ConfigureAwait(false);

        var uri = $"{Request.Path.Value}/{request.Id}";
        return Created(uri, new CreateItemAuditResponse(request.Id));
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetItemsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetItemAuditsAsync()
    {
        var itemAudits = await mediator
            .Send(new GetItemAuditsQuery(), HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return Ok(new GetItemAuditsResponse(itemAudits));
    }
}