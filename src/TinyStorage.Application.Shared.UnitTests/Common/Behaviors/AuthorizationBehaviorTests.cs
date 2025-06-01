namespace TinyStorage.Application.Shared.UnitTests.Common.Behaviors;

public sealed class AuthorizationBehaviorTests
{
    [Fact]
    public async Task Handle_AllAuthorizersSuccess_CallsNext()
    {
        var authorizer = new Mock<IAuthorizer<DummyRequest>>();
        authorizer.Setup(x => x.AuthorizeAsync(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizationResult.Success());

        var behavior = new AuthorizationBehavior<DummyRequest, int>([authorizer.Object]);

        var called = false;

        var result = await behavior.Handle(new DummyRequest(), () =>
        {
            called = true;
            return Task.FromResult(42);
        }, CancellationToken.None);

        result.Should().Be(42);
        called.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_AuthorizerFailed_ThrowsUnauthorizedAccessException()
    {
        var authorizer = new Mock<IAuthorizer<DummyRequest>>();
        authorizer.Setup(x => x.AuthorizeAsync(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizationResult.Failed());

        var behavior = new AuthorizationBehavior<DummyRequest, int>([authorizer.Object]);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            behavior.Handle(new DummyRequest(), () => Task.FromResult(0), CancellationToken.None));
    }

    public sealed record DummyRequest : IRequest<int>;
}
