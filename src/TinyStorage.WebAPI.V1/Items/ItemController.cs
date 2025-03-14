namespace Itmo.TinyStorage.WebAPI.V1.Items;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = ApiGroupNames.Name)]
[Route("v{version:apiVersion}/items")]
public sealed class ItemController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateItemAsync([FromBody] CreateItemRequest request)
    {
        var itemId = await mediator
            .Send(new CreateItemCommand(request.Id, request.Name), HttpContext.RequestAborted)
            .ConfigureAwait(false);
        
        var uri = $"{Request.Path.Value}/{request.Id}";
        return Created(uri, new CreateItemResponse(request.Id, request.Name));
    }
}