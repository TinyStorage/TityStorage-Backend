namespace TinyStorage.Application.Shared.UnitTests.Items.CreateItem;

public sealed class CreateItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesItem_SavesToDb_ReturnsId_AndLogs()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreateItemCommandHandler>>();

        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new TinyStorageContext(options, new UserAccessor(new HttpContextAccessor()));

        var testGuid = Guid.NewGuid();
        var time = DateTime.UtcNow;

        var timeProviderMock = new Mock<TimeProvider>();
        timeProviderMock.Setup(x => x.GetUtcNow()).Returns(new DateTimeOffset(time));

        var handler = new CreateItemCommandHandler(loggerMock.Object, context, timeProviderMock.Object);

        var cmd = new CreateItemCommand(testGuid, "SomeName");

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);
        await context.SaveChangesAsync();

        // Assert
        result.Should().Be(testGuid);

        var entity = await context.Items.SingleAsync(x => x.Id == testGuid);
        entity.Should().NotBeNull();
        entity.Name.Should().Be("SomeName");
        entity.TakenBy.Should().BeNull();
        entity.CreatedAt.Should().Be(time);
        entity.UpdatedAt.Should().Be(time);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Created item")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
