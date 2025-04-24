namespace Itmo.TinyStorage.Application.Shared.Items.Commands;

public sealed record TakeItemCommand(Guid Id) : ICommand;

[UsedImplicitly]
public sealed class TakeItemAuthorizer(ILogger<TakeItemAuthorizer> logger, IUserAccessor user)
{
    public async Task<AuthorizationResult> AuthorizeAsync(TakeItemCommand command, CancellationToken cancellation)
    {
        if (!user.IsLaboratoryAssistant)
        {
            logger.LogInformation("User {Isu} has not role {Role}", user.Isu, "Лаборант");

            return await Task.FromResult(AuthorizationResult.Failed());
        }

        return await Task.FromResult(AuthorizationResult.Success());
    }
}

[UsedImplicitly]
public sealed class TakeItemValidator : AbstractValidator<TakeItemCommand>
{
    public TakeItemValidator()
    {
        RuleFor(command => command.Id)
            .Must(id => id != Guid.Empty)
            .WithMessage("Id is required")
            .WithErrorCode("0001");
    }
}

[UsedImplicitly]
public sealed class TakeItemCommandHandler(
    ILogger<TakeItemCommandHandler> logger,
    IUserAccessor user,
    TinyStorageContext context,
    TimeProvider timeProvider) : ICommandHandler<TakeItemCommand>
{
    private readonly DbSet<ItemModel> _item = context.Items;

    public async Task Handle(TakeItemCommand command, CancellationToken cancellationToken)
    {
        var itemModel = await _item.FindAsync([command.Id], cancellationToken);
        if (itemModel is null)
        {
            throw new ItemInfrastructureException("Item not found");
        }

        var item = new Item(itemModel.Id, itemModel.Name, itemModel.TakenBy);
        item.Take(user.Isu);

        logger.LogInformation("Taken item {Id} with name {Name}", item.Id, item.Name);

        itemModel.Id = item.Id;
        itemModel.Name = item.Name;
        itemModel.TakenBy = item.TakenBy;
        itemModel.CreatedAt = itemModel.CreatedAt;
        itemModel.UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;
    }
}
