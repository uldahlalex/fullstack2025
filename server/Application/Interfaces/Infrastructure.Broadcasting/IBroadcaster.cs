namespace Application.Interfaces.Infrastructure.Broadcasting;

public interface IBroadcaster
{
    void Broadcast(string jsonSerializedMessage, params Guid[] connectionIds);
}