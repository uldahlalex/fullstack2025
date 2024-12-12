using service;
using service.Interfaces;

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