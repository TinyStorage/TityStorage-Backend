namespace TinyStorage.Application.Shared.UnitTests.Items.GiveItem;

public sealed class GiveItemAuthorizerTests
{
    [Fact]
    public async Task AuthorizeAsync_UserIsLaboratoryAssistant_ReturnsSuccess()
    {
        var logger = new Mock<ILogger<GiveItemAuthorizer>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.IsLaboratoryAssistant).Returns(true);

        var sut = new GiveItemAuthorizer(logger.Object, user.Object);
        var cmd = new GiveItemCommand(Guid.NewGuid());

        var result = await sut.AuthorizeAsync(cmd, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task AuthorizeAsync_UserIsNotLaboratoryAssistant_ReturnsFailed_AndLogs()
    {
        var logger = new Mock<ILogger<GiveItemAuthorizer>>();
        var user = new Mock<IUserAccessor>();
        user.SetupGet(u => u.IsLaboratoryAssistant).Returns(false);
        user.SetupGet(u => u.Isu).Returns(222);

        var sut = new GiveItemAuthorizer(logger.Object, user.Object);
        var cmd = new GiveItemCommand(Guid.NewGuid());

        var result = await sut.AuthorizeAsync(cmd, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("User 222 has not role Лаборант")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
