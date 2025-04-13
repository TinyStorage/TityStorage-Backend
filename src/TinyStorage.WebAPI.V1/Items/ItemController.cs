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
        var itemId = await mediator
            .Send(new CreateItemCommand(request.Id, request.Name), HttpContext.RequestAborted)
            .ConfigureAwait(false);

        var uri = $"{Request.Path.Value}/{request.Id}";
        return Created(uri, new CreateItemResponse(request.Id, request.Name));
    }
}