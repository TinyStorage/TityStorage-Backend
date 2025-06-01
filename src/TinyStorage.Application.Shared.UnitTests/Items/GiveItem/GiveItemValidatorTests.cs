namespace TinyStorage.Application.Shared.UnitTests.Items.GiveItem;

public sealed class GiveItemValidatorTests
{
    private readonly GiveItemValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ReturnsValid()
    {
        var command = new GiveItemCommand(Guid.NewGuid());
        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyGuid_ReturnsError()
    {
        var command = new GiveItemCommand(Guid.Empty);
        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == "0001" && e.ErrorMessage == "Id is required");
    }
}
