namespace TinyStorage.Application.Shared.UnitTests.ItemAudits;

public sealed class GetItemAuditsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAllItemAudits_AndLogs()
    {
        // Arrange
        var logger = new Mock<ILogger<GetItemAuditsHandler>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.IsAdministrator).Returns(false);
        user.SetupGet(u => u.Isu).Returns(444);
        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new TinyStorageContext(options, user.Object);

        var item = new ItemModel { Id = Guid.NewGuid(), Name = "TestItem" };
        var audit1 = new ItemAuditModel { Id = Guid.NewGuid(), ItemModel = item, Property = "Name", Value = "A" };
        var audit2 = new ItemAuditModel { Id = Guid.NewGuid(), ItemModel = item, Property = "TakenBy", Value = "1" };

        await context.Items.AddAsync(item);
        await context.ItemAudits.AddRangeAsync(audit1, audit2);
        await context.SaveChangesAsync();

        var handler = new GetItemAuditsHandler(logger.Object, context);
        var query = new GetItemAuditsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(new ItemAuditView(item.Id, "TestItem", "Name", "A"));
        result.Should().ContainEquivalentOf(new ItemAuditView(item.Id, "TestItem", "TakenBy", "1"));

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Get 2 item audits")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyDb_ReturnsEmptyList_AndLogsZero()
    {
        // Arrange
        var logger = new Mock<ILogger<GetItemAuditsHandler>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.IsAdministrator).Returns(false);
        user.SetupGet(u => u.Isu).Returns(444);
        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new TinyStorageContext(options, user.Object);

        var handler = new GetItemAuditsHandler(logger.Object, context);
        var query = new GetItemAuditsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Get 0 item audits")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
