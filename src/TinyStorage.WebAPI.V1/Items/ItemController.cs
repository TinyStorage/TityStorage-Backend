namespace Itmo.TinyStorage.WebAPI.V1.Items;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = ApiGroupNames.Name)]
[Route("v{version:apiVersion}/items")]
public sealed class ItemController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateItemAsync([FromBody] CreateItemRequest request)
    {
        var itemId = await mediator.Send(new CreateItemCommand(request.Id, request.Name), HttpContext.RequestAborted);

        var uri = $"{Request.Path.Value}/{itemId}";
        return Created(uri, new CreateItemResponse(itemId, request.Name));
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetItemsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetItemsAsync()
    {
        var items = await mediator.Send(new GetItemsQuery(), HttpContext.RequestAborted);

        return Ok(new GetItemsResponse(items));
    }

    [HttpPost("{id:guid}/is-taken")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> TakeGiveItemAsync(Guid id, TakeGiveRequest request)
    {
        if (request.IsTaken)
        {
            await mediator.Send(new TakeItemCommand(id), HttpContext.RequestAborted);
        }
        else
        {
            await mediator.Send(new GiveItemCommand(id), HttpContext.RequestAborted);
        }

        return NoContent();
    }
}
