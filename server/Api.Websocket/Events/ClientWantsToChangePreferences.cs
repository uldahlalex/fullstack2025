using Application.Interfaces;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Events;

public class ClientWantsToChangePreferences(IServiceLogic service) : BaseEventHandler<ClientWantsToChangePreferencesDto>
{
    public override Task Handle(ClientWantsToChangePreferencesDto dto, IWebSocketConnection socket)
    {
        service.ChangePreferences(dto); //todo I guess the preference change might as well have been triggered by REST
        return Task.CompletedTask;
    }
}