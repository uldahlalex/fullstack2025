using WebSocketBoilerplate;

namespace Api.Websocket.EventHandlers.ServerEventDtos;

public class ServerSendsEchoDto : BaseDto
{
    public string Message { get; set; }
}