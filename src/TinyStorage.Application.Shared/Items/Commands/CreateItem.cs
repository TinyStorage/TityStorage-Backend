namespace Itmo.TinyStorage.Application.Shared.Items.Commands;

public sealed record CreateItemCommand(Guid Id, string Name) : ICommand<Guid>;

[UsedImplicitly]
public sealed class CreateItemAuthorizer(
    ILogger<CreateItemAuthorizer> logger,
    IUserAccessor user) : IAuthorizer<CreateItemCommand>
{
    public async Task<AuthorizationResult> AuthorizeAsync(CreateItemCommand command, CancellationToken cancellation)
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
public sealed class CreateItemValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemValidator()
    {
        RuleFor(command => command.Id)
            .Must(id => id != Guid.Empty)
            .WithMessage("Id is required")
            .WithErrorCode("0001");

        RuleFor(command => command.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Name is required")
            .WithErrorCode("0002");
    }
}

[UsedImplicitly]
public sealed class CreateItemCommandHandler(
    ILogger<CreateItemCommandHandler> logger) : ICommandHandler<CreateItemCommand, Guid>
{
    public async Task<Guid> Handle(CreateItemCommand command, CancellationToken cancellationToken)
    {
        var item = new Item(command.Id, command.Name);

        logger.LogInformation("Created item {Id} with name {Name}", item.Id, item.Name);

        return item.Id;
    }
}