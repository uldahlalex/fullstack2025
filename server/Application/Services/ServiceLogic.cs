using Application.Interfaces;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
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