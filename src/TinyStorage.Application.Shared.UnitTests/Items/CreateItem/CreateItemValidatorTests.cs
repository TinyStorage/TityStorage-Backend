namespace TinyStorage.Application.Shared.UnitTests.Items.CreateItem;

public sealed class CreateItemValidatorTests
{
    private readonly CreateItemValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var cmd = new CreateItemCommand(Guid.NewGuid(), "test");
        var result = _validator.Validate(cmd);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_InvalidName_ReturnsError(string invalidName)
    {
        var cmd = new CreateItemCommand(Guid.NewGuid(), invalidName);
        var result = _validator.Validate(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorCode == "0002" && x.ErrorMessage == "Name is required");
    }

    [Fact]
    public void Validate_EmptyGuid_ReturnsError()
    {
        var cmd = new CreateItemCommand(Guid.Empty, "valid");
        var result = _validator.Validate(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorCode == "0001" && x.ErrorMessage == "Id is required");
    }
}
