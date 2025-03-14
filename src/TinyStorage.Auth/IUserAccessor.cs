namespace Itmo.TinyStorage.Auth;

public interface IUserAccessor
{
    int Isu { get; }
    bool IsLaboratoryAssistant { get; }
    bool IsAdministrator { get; }
}