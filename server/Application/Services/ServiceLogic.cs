using Api;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Interfaces.Infrastructure.Postgres;
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
    IMqttClientService mqtt
  //  IConnectionManager<IAppConnection> connectionManager
) : IServiceLogic
{
    public async Task Broadcast<T>(string topic, T message)
    {
     //   await connectionManager.BroadcastToTopic(topic, message);
    }

    public async Task JoinTopic(string s, string messages)
    {
     //   connectionManager.AddToTopic(messages, s);
        
    }
}