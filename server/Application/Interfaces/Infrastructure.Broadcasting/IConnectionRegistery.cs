namespace service.Interfaces;

public interface IConnectionRegistry
{
    void RegisterConnection(IConnection connection);
    void UnregisterConnection(IConnection connection);
}