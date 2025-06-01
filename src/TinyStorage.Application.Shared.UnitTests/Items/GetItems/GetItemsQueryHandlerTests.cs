namespace TinyStorage.Application.Shared.UnitTests.Items.GetItems;

public sealed class GetItemsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAllItems_AndLogs()
    {
        // Arrange
        var logger = new Mock<ILogger<GetItemsQueryHandler>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.Isu).Returns(777);
        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new TinyStorageContext(options, user.Object);

        var items = new[]
        {
            new ItemModel { Id = Guid.NewGuid(), Name = "Item1", TakenBy = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ItemModel { Id = Guid.NewGuid(), Name = "Item2", TakenBy = 123, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ItemModel { Id = Guid.NewGuid(), Name = "Item3", TakenBy = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        await context.Items.AddRangeAsync(items);
        await context.SaveChangesAsync();

        var handler = new GetItemsQueryHandler(logger.Object, context);

        var query = new GetItemsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().ContainEquivalentOf(new ItemView(items[0].Id, "Item1", null));
        result.Should().ContainEquivalentOf(new ItemView(items[1].Id, "Item2", 123));
        result.Should().ContainEquivalentOf(new ItemView(items[2].Id, "Item3", null));

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Get 3 items")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyDb_ReturnsEmptyList_AndLogsZero()
    {
        // Arrange
        var logger = new Mock<ILogger<GetItemsQueryHandler>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.Isu).Returns(777);
        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new TinyStorageContext(options, user.Object);

        var handler = new GetItemsQueryHandler(logger.Object, context);

        var query = new GetItemsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Get 0 items")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
