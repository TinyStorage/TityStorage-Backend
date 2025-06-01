namespace TinyStorage.Application.Shared.UnitTests.Common;

public sealed class GenericTypeExtensionsTests
{
    [Fact]
    public void GetGenericTypeName_NonGenericType_ReturnsName()
    {
        typeof(string).GetGenericTypeName().Should().Be("String");
        var value = 123;
        value.GetGenericTypeName().Should().Be("Int32");
    }

    [Fact]
    public void GetGenericTypeName_GenericType_ReturnsFormattedName()
    {
        typeof(List<string>).GetGenericTypeName().Should().Be("List<String>");
        var dict = new Dictionary<int, string>();
        dict.GetGenericTypeName().Should().Be("Dictionary<Int32,String>");
    }
}
