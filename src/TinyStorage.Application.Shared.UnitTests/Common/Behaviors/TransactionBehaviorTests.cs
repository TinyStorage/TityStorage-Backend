namespace TinyStorage.Application.Shared.UnitTests.Common.Behaviors;

public sealed class TransactionBehaviorTests
{
    [Fact]
    public async Task Handle_QueryRequest_CallsNext_NoTransaction()
    {
        var logger = new Mock<ILogger<TransactionBehavior<QueryReq, int>>>();
        var sp = new Mock<IServiceProvider>();

        var behavior = new TransactionBehavior<QueryReq, int>(sp.Object, logger.Object);

        Task<int> Next() => Task.FromResult(77);

        var result = await behavior.Handle(new QueryReq(), Next, CancellationToken.None);

        result.Should().Be(77);
    }

    [Fact]
    public async Task Handle_QueryType_CallsNext_WithoutTransaction()
    {
        var sp = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<TransactionBehavior<QueryReq, int>>>();
        var behavior = new TransactionBehavior<QueryReq, int>(sp.Object, logger.Object);

        var called = false;

        Task<int> Next()
        {
            called = true;
            return Task.FromResult(123);
        }

        var result = await behavior.Handle(new QueryReq(), Next, CancellationToken.None);

        result.Should().Be(123);
        called.Should().BeTrue();
        sp.Verify(x => x.GetService(It.IsAny<Type>()), Times.Never);
        logger.VerifyNoOtherCalls();
    }

    public sealed record Cmd : ICommand<int>;

    public sealed record QueryReq : IQuery<int>;
}
