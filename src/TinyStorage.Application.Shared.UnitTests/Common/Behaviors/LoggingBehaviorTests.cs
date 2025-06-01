public sealed class LoggingBehaviorTests
{
    [Fact]
    public async Task Handle_LogsBeforeAndAfter()
    {
        var logger = new Mock<ILogger<LoggingBehavior<DummyRequest, int>>>();

        var behavior = new LoggingBehavior<DummyRequest, int>(logger.Object);

        Task<int> Next() => Task.FromResult(13);

        var req = new DummyRequest();

        var res = await behavior.Handle(req, Next, CancellationToken.None);

        res.Should().Be(13);
        logger.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2));
    }

    public sealed record DummyRequest : IBaseRequest;
}
