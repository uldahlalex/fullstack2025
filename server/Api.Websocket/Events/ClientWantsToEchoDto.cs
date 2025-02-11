using WebSocketBoilerplate;

namespace Api.Websocket.Events;

public class ClientWantsToEchoDto : BaseDto
{
    public string Message { get; set; }
}