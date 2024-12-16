using Application.Interfaces;
using Application.Interfaces.Infrastructure.Broadcasting;
using Application.Interfaces.Infrastructure.Data;
using Application.Interfaces.Infrastructure.TimeeSeries;
using Application.Models.Entities;

namespace Application.Services;

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