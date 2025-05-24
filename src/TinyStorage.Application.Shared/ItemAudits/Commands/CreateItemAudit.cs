using Itmo.TinyStorage.Domain.Aggregates.ItemAudits;

namespace Itmo.TinyStorage.Application.Shared.ItemAudits.Commands;

public sealed record CreateItemAuditCommand(Guid Id, Guid ItemId, int EditedBy, string Property, string Value) : ICommand<Guid>;

[UsedImplicitly]
public sealed class CreateItemAuditAuthorizer(ILogger<CreateItemAuditAuthorizer> logger, IUserAccessor user)
    : IAuthorizer<CreateItemAuditCommand>
{
    public async Task<AuthorizationResult> AuthorizeAsync(CreateItemAuditCommand command, CancellationToken cancellation)
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
public sealed class CreateItemAuditValidator : AbstractValidator<CreateItemAuditCommand>
{
    public CreateItemAuditValidator()
    {
        RuleFor(command => command.Id)
            .Must(id => id != Guid.Empty)
            .WithMessage("Id is required")
            .WithErrorCode("0001");

        RuleFor(command => command.ItemId)
            .Must(itemId => itemId != Guid.Empty)
            .WithMessage("ItemId is required")
            .WithErrorCode("0002");
        
        RuleFor(command => command.Property)
            .Must(property => !string.IsNullOrWhiteSpace(property))
            .WithMessage("Property is required")
            .WithErrorCode("0003");
        
        RuleFor(command => command.Value)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Value is required")
            .WithErrorCode("0004");
        
        RuleFor(command => command.EditedBy)
            .Must(editedBy => editedBy != default)
            .WithMessage("Value is required")
            .WithErrorCode("0005");
    }
}

[UsedImplicitly]
public sealed class CreateItemAuditCommandHandler(
    ILogger<CreateItemAuditCommandHandler> logger,
    TinyStorageContext context,
    TimeProvider timeProvider) : ICommandHandler<CreateItemAuditCommand, Guid>
{
    private readonly DbSet<ItemAuditModel> _itemAudit = context.ItemAudits;

    public async Task<Guid> Handle(CreateItemAuditCommand command, CancellationToken cancellationToken)
    {
        var itemAudit = new ItemAudit(command.Id, command.ItemId, command.EditedBy, command.Property, command.Value);

        logger.LogInformation("Created item audit {Id} with property {Property} and value {Value} for item {ItemId}", itemAudit.Id, itemAudit.Property, itemAudit.Value, itemAudit.ItemId);

        await _itemAudit.AddAsync(new ItemAuditModel
        {
            Id = itemAudit.Id,
            ItemId = itemAudit.ItemId,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
            EditedBy = itemAudit.EditedBy,
            Property = itemAudit.Property,
            Value = itemAudit.Value,
        }, cancellationToken);

        return itemAudit.Id;
    }
}