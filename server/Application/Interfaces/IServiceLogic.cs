using Application.Models;
using Application.Models.Dtos.Websocket;
using Application.Models.Entities;

namespace Application.Interfaces;

public interface IServiceLogic
{
    public IEnumerable<Board> GetDomainModels(JwtClaims claims);
    public void Broadcast(object message, params string[] topics);

    public void Publish();
    object ChangePreferences(IClientWantsToChangePreferences dto);
}