using service.Models;

namespace service.Interfaces;

public interface IServiceLogic
{
    public IEnumerable<Board> GetDomainModels();
    public void Broadcast(string jsonSerializedMessage, params Guid[] connectionIds);

    public void Publish();
}