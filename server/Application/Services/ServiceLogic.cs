using infrastructure;
using service.Interfaces;
using service.Interfaces.Infrastructure.Broadcasting;
using service.Interfaces.Infrastructure.Data;
using service.Interfaces.Infrastructure.TimeeSeries;

namespace service.Services;

public class ServiceLogic(
    IDataRepository repo,
    ITimeSeriesPublishing timeSeriesPublishing,
    IBroadcaster broadcaster) : IServiceLogic
{
    public IEnumerable<Board> GetDomainModels()
    {
        return repo.GetDomainModels();
    }

    public void Broadcast(string jsonSerializedMessage, params Guid[] connectionIds)
    {
        broadcaster.Broadcast(jsonSerializedMessage, connectionIds);
    }

    public void Publish()
    {
        throw new NotImplementedException();
    }
}