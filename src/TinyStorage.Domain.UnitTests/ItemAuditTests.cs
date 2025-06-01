namespace TinyStorage.Domain.UnitTests;

public sealed class ItemAuditTests
{
    [Fact]
    public void Constructor_SetsAllPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var editedBy = 12345;
        var property = "Name";
        var value = "TestValue";

        // Act
        var audit = new ItemAudit(id, itemId, editedBy, property, value);

        // Assert
        audit.Id.Should().Be(id);
        audit.ItemId.Should().Be(itemId);
        audit.EditedBy.Should().Be(editedBy);
        audit.Property.Should().Be(property);
        audit.Value.Should().Be(value);
    }
}
