using Api;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Application.Models.Entities;

namespace Application.Services;

/// <summary>
/// Exposes concrete service 
/// </summary>
/// <param name="repo"></param>
/// <param name="mqtt"></param>
/// <param name="redisRepo"></param>
public class ServiceLogic(
    IDataRepository repo,
    IMqttClientService mqtt,
        IConnectionManager connectionManager
) : IServiceLogic
{
    public async void BroadcastToTopic(string message, string topic)
    {
       
    }
    
    public async void SubscribeToTopic(string guid, string topic)
    {
        
    }

    public IEnumerable<Board> GetDomainModels(JwtClaims claims)
    {
        return repo.GetDomainModels();
    }

    public void Publish()
    {
    }

    public object ChangePreferences(double temperatureThreshold)
    {
        //Persist new changes to DB
        //Broadcast new preferences to IoT devices in 
        //Broadcast new preferences to web clients
        //messagePublisher.PublishAsync("preferences", dto);
        throw new NotImplementedException();
    }

    public void Connect(object connection)
    {
        
    }


    public async Task Broadcast<T>(string topic, Action<List<T>> broadcastAction)
    {
      //   redisRepo.BroadcastToTopic(topic, broadcastAction);
    }
}