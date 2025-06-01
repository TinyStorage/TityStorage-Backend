namespace TinyStorage.Application.Shared.UnitTests.Items.GiveItem;

public sealed class GiveItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_ItemExists_UpdatesTakenByNull_AndLogs()
    {
        // Arrange
        var logger = new Mock<ILogger<GiveItemCommandHandler>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.Isu).Returns(999);

        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new TinyStorageContext(options, user.Object);

        var testGuid = Guid.NewGuid();
        var initialTime = DateTime.UtcNow.AddDays(-1);
        var now = DateTime.UtcNow;

        var itemModel = new ItemModel
        {
            Id = testGuid,
            Name = "TestItem",
            TakenBy = 999,
            CreatedAt = initialTime,
            UpdatedAt = initialTime
        };
        await context.Items.AddAsync(itemModel);
        await context.SaveChangesAsync();

        var timeProvider = new Mock<TimeProvider>();
        timeProvider.Setup(x => x.GetUtcNow()).Returns(new DateTimeOffset(now));

        var handler = new GiveItemCommandHandler(
            logger.Object,
            user.Object,
            context,
            timeProvider.Object);

        var cmd = new GiveItemCommand(testGuid);

        // ACT
        await handler.Handle(cmd, CancellationToken.None);
        await context.SaveChangesAsync();

        // ASSERT
        var entity = await context.Items.SingleAsync(x => x.Id == testGuid);
        entity.TakenBy.Should().BeNull();
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
        var logger = new Mock<ILogger<GiveItemCommandHandler>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.Isu).Returns(111);

        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new TinyStorageContext(options, user.Object);

        var timeProvider = new Mock<TimeProvider>();

        var handler = new GiveItemCommandHandler(
            logger.Object,
            user.Object,
            context,
            timeProvider.Object);

        var cmd = new GiveItemCommand(Guid.NewGuid());

        // Act
        var act = async () => await handler.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ItemInfrastructureException>()
            .WithMessage("Item not found");
    }

    [Fact]
    public async Task Handle_ItemTakenByOther_ThrowsItemDomainException()
    {
        // Arrange
        var logger = new Mock<ILogger<GiveItemCommandHandler>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.Isu).Returns(555);

        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new TinyStorageContext(options, user.Object);

        var testGuid = Guid.NewGuid();
        var itemModel = new ItemModel
        {
            Id = testGuid,
            Name = "TestItem",
            TakenBy = 888,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Items.AddAsync(itemModel);
        await context.SaveChangesAsync();

        var timeProvider = new Mock<TimeProvider>();

        var handler = new GiveItemCommandHandler(
            logger.Object,
            user.Object,
            context,
            timeProvider.Object);

        var cmd = new GiveItemCommand(testGuid);

        // Act
        var act = async () => await handler.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ItemDomainException>()
            .WithMessage("Item is taken by 888");
    }

    [Fact]
    public async Task Handle_ItemNotTaken_ThrowsItemDomainException()
    {
        // Arrange
        var logger = new Mock<ILogger<GiveItemCommandHandler>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.Isu).Returns(555);

        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new TinyStorageContext(options, user.Object);

        var testGuid = Guid.NewGuid();
        var itemModel = new ItemModel
        {
            Id = testGuid,
            Name = "TestItem",
            TakenBy = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Items.AddAsync(itemModel);
        await context.SaveChangesAsync();

        var timeProvider = new Mock<TimeProvider>();

        var handler = new GiveItemCommandHandler(
            logger.Object,
            user.Object,
            context,
            timeProvider.Object);

        var cmd = new GiveItemCommand(testGuid);

        // Act
        var act = async () => await handler.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ItemDomainException>()
            .WithMessage("Item is not taken");
    }
}
