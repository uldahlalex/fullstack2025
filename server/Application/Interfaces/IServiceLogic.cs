using Application.Models.Dtos.Websocket;
using Application.Models.Entities;

namespace Application.Interfaces;

public interface IServiceLogic
{
    public IEnumerable<Board> GetDomainModels();
    public void Broadcast(object message, params Guid[] connectionIds);

    public void Publish();
    object ChangePreferences(IClientWantsToChangePreferences dto);
}