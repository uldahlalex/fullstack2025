namespace service.Interfaces;

public interface IBroadcaster
{
    void Broadcast(string jsonSerializedMessage, params Guid[] connectionIds);
}