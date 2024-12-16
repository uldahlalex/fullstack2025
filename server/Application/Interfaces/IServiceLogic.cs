using Application.Models.Entities;

namespace Application.Interfaces;

public interface IServiceLogic
{
    public IEnumerable<Board> GetDomainModels();
    public void Broadcast(string jsonSerializedMessage, params Guid[] connectionIds);

    public void Publish();
}