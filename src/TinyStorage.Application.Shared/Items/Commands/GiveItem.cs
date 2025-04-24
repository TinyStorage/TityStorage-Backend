namespace Itmo.TinyStorage.Application.Shared.Items.Commands;

public sealed record GiveItemCommand(Guid Id) : ICommand;

[UsedImplicitly]
public sealed class GiveItemAuthorizer(ILogger<GiveItemAuthorizer> logger, IUserAccessor user)
{
    public async Task<AuthorizationResult> AuthorizeAsync(GiveItemCommand command, CancellationToken cancellation)
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
public sealed class GiveItemValidator : AbstractValidator<GiveItemCommand>
{
    public GiveItemValidator()
    {
        RuleFor(command => command.Id)
            .Must(id => id != Guid.Empty)
            .WithMessage("Id is required")
            .WithErrorCode("0001");
    }
}

[UsedImplicitly]
public sealed class GiveItemCommandHandler(
    ILogger<GiveItemCommandHandler> logger,
    IUserAccessor user,
    TinyStorageContext context,
    TimeProvider timeProvider) : ICommandHandler<GiveItemCommand>
{
    private readonly DbSet<ItemModel> _item = context.Items;

    public async Task Handle(GiveItemCommand command, CancellationToken cancellationToken)
    {
        var itemModel = await _item.FindAsync([command.Id], cancellationToken);
        if (itemModel is null)
        {
            throw new ItemInfrastructureException("Item not found");
        }

        var item = new Item(itemModel.Id, itemModel.Name, itemModel.TakenBy);
        item.Give(user.Isu);

        logger.LogInformation("Taken item {Id} with name {Name}", item.Id, item.Name);

        itemModel.Id = item.Id;
        itemModel.Name = item.Name;
        itemModel.TakenBy = item.TakenBy;
        itemModel.CreatedAt = itemModel.CreatedAt;
        itemModel.UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;
    }
}
