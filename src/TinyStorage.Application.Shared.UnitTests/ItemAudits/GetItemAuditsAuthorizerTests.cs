namespace TinyStorage.Application.Shared.UnitTests.ItemAudits;

public sealed class GetItemAuditsAuthorizerTests
{
    [Fact]
    public async Task AuthorizeAsync_UserIsAdmin_ReturnsSuccess()
    {
        var logger = new Mock<ILogger<GetItemAuditsAuthorizer>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.IsAdministrator).Returns(true);

        var sut = new GetItemAuditsAuthorizer(logger.Object, user.Object);
        var query = new GetItemAuditsQuery();

        var result = await sut.AuthorizeAsync(query, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task AuthorizeAsync_UserIsNotAdmin_ReturnsFailed_AndLogs()
    {
        var logger = new Mock<ILogger<GetItemAuditsAuthorizer>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.IsAdministrator).Returns(false);
        user.SetupGet(u => u.Isu).Returns(444);

        var sut = new GetItemAuditsAuthorizer(logger.Object, user.Object);
        var query = new GetItemAuditsQuery();

        var result = await sut.AuthorizeAsync(query, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("User 444 has not role Администратор")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
