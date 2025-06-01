namespace Itmo.TinyStorage.Application.Shared.Items.Commands;

public sealed record DeleteItemCommand(Guid Id) : ICommand;

[UsedImplicitly]
public sealed class DeleteItemAuthorizer(ILogger<DeleteItemAuthorizer> logger, IUserAccessor user)
    : IAuthorizer<DeleteItemCommand>
{
    public async Task<AuthorizationResult> AuthorizeAsync(DeleteItemCommand command, CancellationToken cancellation)
    {
        if (!user.IsAdministrator)
        {
            logger.LogInformation("User {Isu} has not role {Role}", user.Isu, "Администратор");

            return await Task.FromResult(AuthorizationResult.Failed());
        }

        return await Task.FromResult(AuthorizationResult.Success());
    }
}

[UsedImplicitly]
public sealed class DeleteItemValidator : AbstractValidator<DeleteItemCommand>
{
    public DeleteItemValidator()
    {
        RuleFor(command => command.Id)
            .Must(id => id != Guid.Empty)
            .WithMessage("Id is required")
            .WithErrorCode("0001");
    }
}

[UsedImplicitly]
public sealed class DeleteItemCommandHandler(
    ILogger<DeleteItemCommandHandler> logger,
    TinyStorageContext context) : ICommandHandler<DeleteItemCommand>
{
    private readonly DbSet<ItemModel> _item = context.Items;

    public async Task Handle(DeleteItemCommand command, CancellationToken cancellationToken)
    {
        var itemModel = await _item.FindAsync([command.Id], cancellationToken);
        if (itemModel is null)
        {
            throw new ItemInfrastructureException("Item not found");
        }

        logger.LogInformation("Deleted item {Id} with name {Name}", itemModel.Id, itemModel.Name);

        _item.Remove(itemModel);
    }
}
