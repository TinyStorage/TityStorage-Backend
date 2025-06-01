namespace TinyStorage.Application.Shared.UnitTests.Common;

public sealed class ValidationDomainExceptionTests
{
    [Fact]
    public void ValidationDomainException_StoresMessageAndInnerException()
    {
        var inner = new Exception("fail");
        var ex = new ValidationDomainException("err", inner);

        ex.Message.Should().Be("err");
        ex.InnerException.Should().Be(inner);
    }
}
