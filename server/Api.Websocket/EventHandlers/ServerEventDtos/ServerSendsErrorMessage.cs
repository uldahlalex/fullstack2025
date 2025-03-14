using System.ComponentModel.DataAnnotations;
using WebSocketBoilerplate;

namespace Api.Websocket.EventHandlers.ServerEventDtos;

public class ServerSendsErrorMessage : BaseDto
{
    public string Error { get; set; } = null!;

    public string RequestId { get; set; } = null!;
    public string Message { get; set; }
}