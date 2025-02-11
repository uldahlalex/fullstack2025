using WebSocketBoilerplate;

namespace Api.Websocket.EventHandlers.ClientEventDtos;

public class ClientWantsToChangePreferencesDto : BaseDto
{
    public double TemperatureThreshold { get; }
}