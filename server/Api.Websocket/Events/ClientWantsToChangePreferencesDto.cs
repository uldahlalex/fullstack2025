using Application.Models.Dtos.Websocket;
using WebSocketBoilerplate;

namespace Api.Websocket.Events;

public class ClientWantsToChangePreferencesDto : BaseDto, IClientWantsToChangePreferences
{
    public double TemperatureThreshold { get; }
}