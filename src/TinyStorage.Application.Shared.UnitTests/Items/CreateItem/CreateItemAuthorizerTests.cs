namespace TinyStorage.Application.Shared.UnitTests.Items.CreateItem;

public sealed class CreateItemAuthorizerTests
{
    [Fact]
    public async Task AuthorizeAsync_UserIsAdmin_ReturnsSuccess()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreateItemAuthorizer>>();
        var userMock = new Mock<IUserAccessor>();
        userMock.SetupGet(u => u.IsAdministrator).Returns(true);

        var authorizer = new CreateItemAuthorizer(loggerMock.Object, userMock.Object);

        var cmd = new CreateItemCommand(Guid.NewGuid(), "item");

        // Act
        var result = await authorizer.AuthorizeAsync(cmd, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task AuthorizeAsync_UserIsNotAdmin_ReturnsFailed_AndLogs()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreateItemAuthorizer>>();
        var userMock = new Mock<IUserAccessor>();
        userMock.SetupGet(u => u.IsAdministrator).Returns(false);
        userMock.SetupGet(u => u.Isu).Returns(456);

        var authorizer = new CreateItemAuthorizer(loggerMock.Object, userMock.Object);

        var cmd = new CreateItemCommand(Guid.NewGuid(), "item");

        // Act
        var result = await authorizer.AuthorizeAsync(cmd, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("User 456 has not role Администратор")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
