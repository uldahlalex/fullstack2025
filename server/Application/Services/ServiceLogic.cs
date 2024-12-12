using service.Interfaces;
using service.Models;

namespace service.Services;

public class ServiceLogic(IDataRepository repo, 
    ITimeSeriesPublishing timeSeriesPublishing, 
    IBroadcaster broadcaster) : IServiceLogic
{
    public IEnumerable<Board> GetDomainModels()
    {
        return repo.GetDomainModels();
    }

    public void Broadcast(string jsonSerializedMessage, params Guid[] connectionIds)
    {
        timeSeriesPublishing.Publish();
    }

    public void Publish()
    {
        throw new NotImplementedException();
    }

    public void Broadcast(List<Guid> connectionIds, string jsonSerializedMessage)
    {
        broadcaster.Broadcast(connectionIds, jsonSerializedMessage);
    }

  
}