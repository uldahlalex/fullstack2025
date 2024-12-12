namespace service.Interfaces.Infrastructure.Broadcasting;

public interface IConnectionRegistry
{
    void RegisterConnection(IConnection connection);
    void UnregisterConnection(IConnection connection);
}