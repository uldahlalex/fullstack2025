using WebSocketBoilerplate;

namespace Api.Websocket.Events;

public class ServerSendsEchoDto : BaseDto
{
    public string Message { get; set; }
}