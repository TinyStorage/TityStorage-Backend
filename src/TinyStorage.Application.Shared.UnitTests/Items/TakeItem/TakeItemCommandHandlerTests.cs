namespace TinyStorage.Application.Shared.UnitTests.Items.TakeItem;

public sealed class TakeItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_ItemExists_UpdatesTakenBy_AndLogs()
    {
        // Arrange
        var logger = new Mock<ILogger<TakeItemCommandHandler>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.Isu).Returns(777);

        var interceptor = new UpdateAuditableInterceptor(user.Object);

        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(interceptor)
            .Options;
        await using var context = new TinyStorageContext(options, user.Object);

        var testGuid = Guid.NewGuid();
        var initialTime = DateTime.UtcNow.AddDays(-1);
        var now = DateTime.UtcNow;

        var itemModel = new ItemModel
        {
            Id = testGuid,
            Name = "TestItem",
            TakenBy = null,
            CreatedAt = initialTime,
            UpdatedAt = initialTime
        };
        await context.Items.AddAsync(itemModel);
        await context.SaveChangesAsync();

        var timeProvider = new Mock<TimeProvider>();
        timeProvider.Setup(x => x.GetUtcNow()).Returns(new DateTimeOffset(now));

        var handler = new TakeItemCommandHandler(
            logger.Object,
            user.Object,
            context,
            timeProvider.Object);

        var cmd = new TakeItemCommand(testGuid);

        // Act
        await handler.Handle(cmd, CancellationToken.None);
        await context.SaveChangesAsync();

        // Assert
        var entity = await context.Items.SingleAsync(x => x.Id == testGuid);
        entity.TakenBy.Should().Be(777);
        entity.Name.Should().Be("TestItem");
        entity.CreatedAt.Should().Be(initialTime);
        entity.UpdatedAt.Should().Be(now);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Taken item")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ItemNotFound_ThrowsItemInfrastructureException()
    {
        // Arrange
        var logger = new Mock<ILogger<TakeItemCommandHandler>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.Isu).Returns(123);

        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new TinyStorageContext(options, new UserAccessor(new HttpContextAccessor()));

        var timeProvider = new Mock<TimeProvider>();

        var handler = new TakeItemCommandHandler(
            logger.Object,
            user.Object,
            context,
            timeProvider.Object);

        var cmd = new TakeItemCommand(Guid.NewGuid());

        // Act
        var act = async () => await handler.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ItemInfrastructureException>()
            .WithMessage("Item not found");
    }
}
