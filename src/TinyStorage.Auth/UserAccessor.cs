namespace Itmo.TinyStorage.Auth;

public sealed class UserAccessor(IHttpContextAccessor httpContextAccessor) : IUserAccessor
{
    private readonly ClaimsPrincipal _principal = httpContextAccessor.HttpContext?.User!;

    public int Isu
    {
        get
        {
            var claim = _principal.Claims.FirstOrDefault(claim => claim.Type == "isu");
            return claim is not null ? int.Parse(claim.Value) : 0;
        }
    }

    public bool IsLaboratoryAssistant => _principal.Claims
        .Any(claim => claim is { Type: ClaimTypes.Role, Value: "Лаборант" });

    public bool IsAdministrator => _principal.Claims
        .Any(claim => claim is { Type: ClaimTypes.Role, Value: "Администратор" });
}