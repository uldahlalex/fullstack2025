using service.Interfaces;

namespace Infrastructure.Websocket;


public class Broadcaster(IState state) : IBroadcaster
{

    public void Broadcast(string jsonSerializedMessage, params Guid[] connectionIds)
    {
        foreach (var connectionId in connectionIds)
        {
            var currentState = state.Connections;
            state.Connections[connectionId].Send(jsonSerializedMessage);
        }    
    }
}
