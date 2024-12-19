using System.Text.Json;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models.Dtos.Websocket;
using Application.Models.Entities;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Application.Services;

public class ServiceLogic(
    IDataRepository repo,
    IMqttPublisher mqttPublisher,
    IWebsocketClientMessager websocketClientMessager) : IServiceLogic
{
    public IEnumerable<Board> GetDomainModels()
    {
        return repo.GetDomainModels();
    }

    public void Broadcast(object message, params Guid[] connectionIds)
    {
        var serializedMessage = JsonSerializer.Serialize(message);
        websocketClientMessager.Broadcast(serializedMessage, connectionIds);
    }

    public void Publish()
    {
        throw new NotImplementedException();
    }

    public object ChangePreferences(IClientWantsToChangePreferences dto)
    {
        //Persist new changes to DB
        //Broadcast new preferences to IoT devices in 
        //Broadcast new preferences to web clients
        throw new NotImplementedException();
    }
}