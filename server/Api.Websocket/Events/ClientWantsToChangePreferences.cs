using Application.Interfaces;
using Application.Models.Dtos.Websocket;
using Fleck;
using lib;

namespace Api.Websocket.Events;

public class ClientWantsToChangePreferencesDto : BaseDto, IClientWantsToChangePreferences
{
    public double TemperatureThreshold { get; }
}

public class ClientWantsToChangePreferences(IServiceLogic service) : BaseEventHandler<ClientWantsToChangePreferencesDto>
{
    public override Task Handle(ClientWantsToChangePreferencesDto dto, IWebSocketConnection socket)
    {
        service.ChangePreferences(dto); //todo I guess the preference change might as well have been triggered by REST
        return Task.CompletedTask;
    }
}