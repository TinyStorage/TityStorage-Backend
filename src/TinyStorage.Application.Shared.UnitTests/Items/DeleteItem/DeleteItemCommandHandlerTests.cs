namespace TinyStorage.Application.Shared.UnitTests.Items.DeleteItem;

public sealed class DeleteItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_ItemExists_RemovesFromDb_AndLogs()
    {
        // Arrange
        var logger = new Mock<ILogger<DeleteItemCommandHandler>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.Isu).Returns(777);
        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new TinyStorageContext(options, user.Object);

        var testGuid = Guid.NewGuid();
        var itemModel = new ItemModel
        {
            Id = testGuid,
            Name = "DeleteMe",
            TakenBy = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Items.AddAsync(itemModel);
        await context.SaveChangesAsync();

        var handler = new DeleteItemCommandHandler(logger.Object, context);

        var cmd = new DeleteItemCommand(testGuid);

        // ACT
        await handler.Handle(cmd, CancellationToken.None);
        await context.SaveChangesAsync();

        // ASSERT
        (await context.Items.AnyAsync(x => x.Id == testGuid)).Should().BeFalse();

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Deleted item")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ItemNotFound_ThrowsItemInfrastructureException()
    {
        // Arrange
        var logger = new Mock<ILogger<DeleteItemCommandHandler>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.Isu).Returns(777);
        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new TinyStorageContext(options, user.Object);

        var handler = new DeleteItemCommandHandler(logger.Object, context);
        var cmd = new DeleteItemCommand(Guid.NewGuid());

        // ACT
        var act = async () => await handler.Handle(cmd, CancellationToken.None);

        // ASSERT
        await act.Should().ThrowAsync<ItemInfrastructureException>()
            .WithMessage("Item not found");
    }
}
