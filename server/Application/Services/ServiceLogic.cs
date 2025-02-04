using System.Text.Json;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models.Dtos.Websocket;
using Application.Models.Entities;


namespace Application.Services;

public class ServiceLogic<T>(
    IDataRepository repo,
    IMqttClientService mqtt,
    IWebSocketService<T> ws) : IServiceLogic
{
    public IEnumerable<Board> GetDomainModels()
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