namespace TinyStorage.Application.Shared.UnitTests.Items.DeleteItem;

public sealed class DeleteItemAuthorizerTests
{
    [Fact]
    public async Task AuthorizeAsync_UserIsAdmin_ReturnsSuccess()
    {
        var logger = new Mock<ILogger<DeleteItemAuthorizer>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.IsAdministrator).Returns(true);

        var sut = new DeleteItemAuthorizer(logger.Object, user.Object);
        var cmd = new DeleteItemCommand(Guid.NewGuid());

        var result = await sut.AuthorizeAsync(cmd, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task AuthorizeAsync_UserIsNotAdmin_ReturnsFailed_AndLogs()
    {
        var logger = new Mock<ILogger<DeleteItemAuthorizer>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.IsAdministrator).Returns(false);
        user.SetupGet(u => u.Isu).Returns(333);

        var sut = new DeleteItemAuthorizer(logger.Object, user.Object);
        var cmd = new DeleteItemCommand(Guid.NewGuid());

        var result = await sut.AuthorizeAsync(cmd, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("User 333 has not role Администратор")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
