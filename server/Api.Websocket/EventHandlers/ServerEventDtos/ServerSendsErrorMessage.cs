using WebSocketBoilerplate;

namespace Api.Websocket.EventHandlers.ServerEventDtos;

public class ServerSendsErrorMessage : BaseDto
{
    public string Error { get; set; }
    public string RequestId { get; set; }
}