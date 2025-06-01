namespace TinyStorage.Domain.UnitTests;

public sealed class ItemTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Test Item";

        // Act
        var item = new Item(id, name);

        // Assert
        item.Id.Should().Be(id);
        item.Name.Should().Be(name);
        item.TakenBy.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithTakenBy_SetsProperties()
    {
        var id = Guid.NewGuid();
        var name = "Test Item";
        var takenBy = 123;

        var item = new Item(id, name, takenBy);

        item.Id.Should().Be(id);
        item.Name.Should().Be(name);
        item.TakenBy.Should().Be(takenBy);
    }

    [Fact]
    public void Take_WhenNotTaken_SetsTakenBy()
    {
        var item = new Item(Guid.NewGuid(), "Test");

        item.Take(456);

        item.TakenBy.Should().Be(456);
    }

    [Fact]
    public void Take_WhenAlreadyTaken_Throws()
    {
        var item = new Item(Guid.NewGuid(), "Test", 100);

        var act = () => item.Take(200);

        act.Should()
            .Throw<ItemDomainException>()
            .WithMessage("Item is already taken by 100");
    }

    [Fact]
    public void Give_WhenNotTaken_Throws()
    {
        var item = new Item(Guid.NewGuid(), "Test");

        var act = () => item.Give(123);

        act.Should()
            .Throw<ItemDomainException>()
            .WithMessage("Item is not taken");
    }

    [Fact]
    public void Give_WhenTakenByDifferentUser_Throws()
    {
        var item = new Item(Guid.NewGuid(), "Test", 123);

        var act = () => item.Give(456);

        act.Should()
            .Throw<ItemDomainException>()
            .WithMessage("Item is taken by 123");
    }

    [Fact]
    public void Give_WhenTakenBySameUser_SetsTakenByNull()
    {
        var item = new Item(Guid.NewGuid(), "Test", 789);

        item.Give(789);

        item.TakenBy.Should().BeNull();
    }
}
