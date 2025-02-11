using WebSocketBoilerplate;

namespace Api.Websocket;

public class ServerSendsErrorMessage : BaseDto
{
    public string Error { get; set; }
    public string RequestId { get; set; }
}