namespace Itmo.TinyStorage.WebAPI.V1.ItemAudits;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = ApiGroupNames.Name)]
[Route("v{version:apiVersion}/item-audits")]
public class ItemAuditController(IMediator mediator) : ControllerBase
{
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
