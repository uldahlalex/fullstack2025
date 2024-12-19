namespace Application.Interfaces.Infrastructure.Websocket;

public interface IBroadcaster
{
    void Broadcast(string jsonSerializedMessage, params Guid[] connectionIds);
}