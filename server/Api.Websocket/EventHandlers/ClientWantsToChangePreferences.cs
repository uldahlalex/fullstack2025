using Api.Websocket.EventHandlers.ClientEventDtos;
using Application.Interfaces;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.EventHandlers;

public class ClientWantsToChangePreferences(IServiceLogic service) : BaseEventHandler<ClientWantsToChangePreferencesDto>
{
    public override Task Handle(ClientWantsToChangePreferencesDto dto, IWebSocketConnection socket)
    {
        service.ChangePreferences(dto.TemperatureThreshold);
        return Task.CompletedTask;
    }
}