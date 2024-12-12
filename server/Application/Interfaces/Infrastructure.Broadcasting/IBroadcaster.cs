namespace service.Interfaces;

public interface IBroadcaster
{
    void Broadcast(List<Guid> connectionIds, string jsonSerializedMessage);
}