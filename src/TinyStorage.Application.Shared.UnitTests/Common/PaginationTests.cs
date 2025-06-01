namespace TinyStorage.Application.Shared.UnitTests.Common;

public sealed class PaginationTests
{
    [Fact]
    public void Pagination_CanBeConstructed_AndPropertiesAccessible()
    {
        var pag = new Pagination(1, 50, 125);
        pag.Offset.Should().Be(1);
        pag.Limit.Should().Be(50);
        pag.Total.Should().Be(125);
    }
}
