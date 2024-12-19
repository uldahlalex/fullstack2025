using Application.Interfaces.Infrastructure.Websocket;

namespace Infrastructure.Websocket;

public class ConnectionRegistry(IState state) : IConnectionRegistry
{
    public void RegisterConnection(IConnection connection)
    {
        state.TryAddConnection(connection);
    }

    public void UnregisterConnection(IConnection connection)
    {
        state.TryRemoveConnection(connection);
    }
}