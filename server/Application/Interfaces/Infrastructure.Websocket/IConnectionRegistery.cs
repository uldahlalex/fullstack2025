namespace Application.Interfaces.Infrastructure.Websocket;

public interface IConnectionRegistry
{
    void RegisterConnection(IConnection connection);
    void UnregisterConnection(IConnection connection);
}