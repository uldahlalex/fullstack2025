using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Application.Models.Dtos.Websocket;
using Application.Models.Entities;

namespace Application.Services;

public class ServiceLogic<W>(
    IDataRepository repo,
    IMqttClientService mqtt,
    IWebSocketService<W> ws) : IServiceLogic
{
    public IEnumerable<Board> GetDomainModels(JwtClaims claims)
    {
        return repo.GetDomainModels();
    }

    public void Broadcast(object message, params string[] topics)
    {
        //ws.broadcast
    }

    public void Publish()
    {
        //mqtt.publish
    }

    public object ChangePreferences(IClientWantsToChangePreferences dto)
    {
        //Persist new changes to DB
        //Broadcast new preferences to IoT devices in 
        //Broadcast new preferences to web clients
        //messagePublisher.PublishAsync("preferences", dto);
        throw new NotImplementedException();
    }
}