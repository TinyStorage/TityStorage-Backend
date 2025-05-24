namespace Itmo.TinyStorage.Application.Extensions;

public sealed record ErrorResponse
{
    private readonly DateTime _createdAt = DateTime.UtcNow;
    private string? _errorId;

    public string ErrorId =>
        _errorId ??= ComputeHash(
                ErrorMessage,
                ErrorCode,
                JsonSerializer.Serialize(ErrorDetails ?? ""),
                _createdAt.ToString("yyyy-MM-dd HH:mm"))
            .ToString("x8");

    public string ErrorCode { get; init; } = "";
    public string ErrorMessage { get; init; } = "";


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? ErrorDetails { get; init; }

    private static ulong ComputeHash(params object[] hashFields)
    {
        var hash = 3074457345618258791ul;

        unchecked
        {
            foreach (var obj in hashFields)
            {
                hash += (ulong)obj.GetHashCode();
                hash *= 3074457345618258799ul;
            }
        }

        return hash;
    }
}
