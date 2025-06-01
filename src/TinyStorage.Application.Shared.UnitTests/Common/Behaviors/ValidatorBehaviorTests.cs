namespace TinyStorage.Application.Shared.UnitTests.Common.Behaviors;

public sealed class ValidatorBehaviorTests
{
    [Fact]
    public async Task Handle_NoValidators_CallsNext()
    {
        var logger = new Mock<ILogger<ValidatorBehavior<DummyRequest, int>>>();
        var behavior = new ValidatorBehavior<DummyRequest, int>(new List<IValidator<DummyRequest>>(), logger.Object);

        Task<int> Next() => Task.FromResult(21);

        var result = await behavior.Handle(new DummyRequest(), Next, CancellationToken.None);

        result.Should().Be(21);
    }

    [Fact]
    public async Task Handle_ValidatorFails_ThrowsValidationDomainException()
    {
        var logger = new Mock<ILogger<ValidatorBehavior<DummyRequest, int>>>();

        var validator = new Mock<IValidator<DummyRequest>>();
        validator.Setup(x => x.Validate(It.IsAny<DummyRequest>()))
            .Returns(new FluentValidation.Results.ValidationResult([
                new FluentValidation.Results.ValidationFailure("Field", "msg") { ErrorCode = "X01" }
            ]));

        var behavior = new ValidatorBehavior<DummyRequest, int>([validator.Object], logger.Object);

        await Assert.ThrowsAsync<ValidationDomainException>(() =>
            behavior.Handle(new DummyRequest(), () => Task.FromResult(0), CancellationToken.None));
    }

    public sealed record DummyRequest : IBaseRequest;
}
