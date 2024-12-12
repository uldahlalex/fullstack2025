using service.Interfaces;

namespace Infrastructure.Websocket;


public class Broadcaster(IState state) : IBroadcaster
{
    public void Broadcast(List<Guid> connectionIds, string jsonSerializedMessage)
    {
        foreach (var connectionId in connectionIds)
        {
            state.Connections[connectionId].Send(jsonSerializedMessage);
        }
    }
}
