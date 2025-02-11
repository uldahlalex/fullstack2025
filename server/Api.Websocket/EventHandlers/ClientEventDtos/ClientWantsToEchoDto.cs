using WebSocketBoilerplate;

namespace Api.Websocket.EventHandlers.ClientEventDtos;

public class ClientWantsToEchoDto : BaseDto
{
    public string Message { get; set; }
}