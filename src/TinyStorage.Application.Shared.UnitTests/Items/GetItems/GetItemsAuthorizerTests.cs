namespace TinyStorage.Application.Shared.UnitTests.Items.GetItems;

public sealed class GetItemsAuthorizerTests
{
    [Fact]
    public async Task AuthorizeAsync_UserIsLaboratoryAssistant_ReturnsSuccess()
    {
        var logger = new Mock<ILogger<GetItemsAuthorizer>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.IsLaboratoryAssistant).Returns(true);

        var sut = new GetItemsAuthorizer(logger.Object, user.Object);
        var query = new GetItemsQuery();

        var result = await sut.AuthorizeAsync(query, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task AuthorizeAsync_UserIsNotLaboratoryAssistant_ReturnsFailed_AndLogs()
    {
        var logger = new Mock<ILogger<GetItemsAuthorizer>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.IsLaboratoryAssistant).Returns(false);
        user.SetupGet(u => u.Isu).Returns(112);

        var sut = new GetItemsAuthorizer(logger.Object, user.Object);
        var query = new GetItemsQuery();

        var result = await sut.AuthorizeAsync(query, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("User 112 has not role Лаборант")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
