using Application.Models;
using Application.Models.Entities;

namespace Application.Interfaces;

public interface IServiceLogic
{

    public Task Broadcast<T>(string topic, Action<List<T>> broadcastAction);
}