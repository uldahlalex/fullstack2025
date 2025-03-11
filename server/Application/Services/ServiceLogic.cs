using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;

namespace Application.Services;

/// <summary>
///     Exposes concrete service
/// </summary>
/// <param name="repo"></param>
/// <param name="mqtt"></param>
/// <param name="redisRepo"></param>
public class ServiceLogic(
    IDataRepository repo
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